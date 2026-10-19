using FinaccApi.DataAccess;
using FinaccApi.Modules.Access.Domain;
using FinaccApi.Modules.Categories;
using FinaccApi.Modules.Categories.Api;
using FinaccApi.Modules.Categories.Application;
using FinaccApi.Modules.Categories.Application.UpdateCategory;
using FinaccApi.Modules.Categories.Domain;
using FinaccApi.Shared.Application.Auth;
using FinaccApi.Shared.Application.MultiTenancy;
using FinaccApi.Shared.Kernel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace FinaccApi.Tests.Modules.Categories.Application.UpdateCategory;

/// <summary>
/// Тесты обработчика обновления категории.
/// </summary>
[TestClass]
public sealed class UpdateCategoryHandlerTests
{
	/// <summary>
	/// Идентификатор организации текущего пользователя.
	/// </summary>
	private const int OrganizationId = 7;

	/// <summary>
	/// Идентификатор пользователя, создавшего категорию.
	/// </summary>
	private const int CreatorId = 22;

	/// <summary>
	/// Идентификатор пользователя, обновляющего категорию.
	/// </summary>
	private const int CurrentUserId = 11;

	/// <summary>
	/// Контекст текущего пользователя.
	/// </summary>
	private ICurrentUser CurrentUser { get; set; } = null!;

	/// <summary>
	/// Контекст организации с проверкой предварительной загрузки.
	/// </summary>
	private ICurrentOrganization CurrentOrganization { get; set; } = null!;

	/// <summary>
	/// Сервис проверки прав текущего пользователя.
	/// </summary>
	private IPermissionChecker PermissionChecker { get; set; } = null!;

	/// <summary>
	/// Инициализирует тест.
	/// </summary>
	[TestInitialize]
	public void Initialize()
	{
		CurrentUser = Substitute.For<ICurrentUser>();
		CurrentUser.RequiredUserId.Returns(CurrentUserId);

		CurrentOrganization = Substitute.For<ICurrentOrganization>();
		CurrentOrganization.OrganizationId.Returns((int?)OrganizationId);

		var organizationContextLoaded = false;
		CurrentOrganization.LoadContext(Arg.Any<CancellationToken>()).Returns(call =>
		{
			call.Arg<CancellationToken>().ThrowIfCancellationRequested();
			organizationContextLoaded = true;
			return Task.CompletedTask;
		});

		CurrentOrganization.HasOrganization.Returns(_ => organizationContextLoaded
			? CurrentOrganization.OrganizationId.HasValue
			: throw new InvalidOperationException("Organization context is not loaded."));

		PermissionChecker = Substitute.For<IPermissionChecker>();
		PermissionChecker
			.HasPermission(Permissions.Categories.Update, Arg.Any<CancellationToken>())
			.Returns(Task.FromResult(true));
	}

	/// <summary>
	/// Проверяет разрешение зависимостей через модуль и возврат HTTP 400 для некорректного ID из маршрута.
	/// </summary>
	[TestMethod]
	public async Task Update_WhenResolvedThroughModule_ReturnsValidationErrorForRouteId()
	{
		var services = new ServiceCollection();
		services.AddDbContext<ApplicationDbContext>(options =>
			options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
		services.AddScoped<ICurrentUser>(_ => CurrentUser);
		services.AddScoped<ICurrentOrganization>(_ => CurrentOrganization);
		services.AddScoped<IPermissionChecker>(_ => PermissionChecker);
		services.AddCategoriesModule(new ConfigurationBuilder().Build());
		await using var provider = services.BuildServiceProvider();
		await using var scope = provider.CreateAsyncScope();
		var controller = ActivatorUtilities.CreateInstance<CategoriesController>(scope.ServiceProvider);

		var result = await controller.Update(
			0, new UpdateCategoryRequest("Updated", null), CancellationToken.None);

		Assert.IsInstanceOfType(result, typeof(ObjectResult));
		var response = (ObjectResult)result;
		Assert.AreEqual(400, response.StatusCode);
		Assert.IsInstanceOfType(response.Value, typeof(ProblemDetails));
		Assert.AreEqual("Category.Validation", ((ProblemDetails)response.Value!).Title);
	}

	/// <summary>
	/// Проверяет обновление данных и аудита категории своей организации с сохранением типа и родителя.
	/// </summary>
	[TestMethod]
	public async Task Handle_WhenCategoryBelongsToOrganization_UpdatesDetailsAndAudit()
	{
		await using var context = CreateContext();
		var parentCategory = await AddCategory(context);
		var category = await AddCategory(context, parentId: parentCategory.Id);
		var createdAt = category.CreatedAt;
		var beforeUpdate = DateTime.UtcNow;
		var handler = CreateHandler(context);

		var result = await handler.Handle(
			new(category.Id, "Updated", "Updated description"), CancellationToken.None);

		Assert.IsTrue(result.IsSuccess);
		Assert.AreEqual(category.Id, result.Value.Id);
		Assert.AreEqual("Updated", result.Value.Name);
		Assert.AreEqual("Updated description", result.Value.Description);
		Assert.AreEqual(OrganizationId, result.Value.OrganizationId);
		Assert.AreEqual(parentCategory.Id, result.Value.ParentCategoryId);
		Assert.AreEqual(CategoryType.Expense, result.Value.CategoryType);
		Assert.AreEqual(ActivityType.Operating, result.Value.ActivityType);

		context.ChangeTracker.Clear();
		var savedCategory = await context.Categories.SingleAsync(c => c.Id == category.Id);

		Assert.AreEqual("Updated", savedCategory.Name);
		Assert.AreEqual("Updated description", savedCategory.Description);
		Assert.AreEqual(CreatorId, savedCategory.CreatedBy);
		Assert.AreEqual(createdAt, savedCategory.CreatedAt);
		Assert.AreEqual(CurrentUserId, savedCategory.UpdatedBy);
		Assert.IsTrue(savedCategory.UpdatedAt >= beforeUpdate);
		Assert.IsTrue(savedCategory.UpdatedAt <= DateTime.UtcNow);
		Assert.AreEqual(2, await context.Categories.CountAsync());
		Assert.AreEqual("Original", (await context.Categories.SingleAsync(c => c.Id == parentCategory.Id)).Name);
	}

	/// <summary>
	/// Проверяет очистку описания при передаче null.
	/// </summary>
	[TestMethod]
	public async Task Handle_WhenDescriptionIsNull_ClearsDescription()
	{
		await using var context = CreateContext();
		var category = await AddCategory(context);
		var handler = CreateHandler(context);

		var result = await handler.Handle(
			new(category.Id, "Updated", null), CancellationToken.None);

		Assert.IsTrue(result.IsSuccess);
		Assert.IsNull(result.Value.Description);
		context.ChangeTracker.Clear();
		Assert.IsNull((await context.Categories.SingleAsync()).Description);
	}

	/// <summary>
	/// Проверяет сохранение названия и описания максимально допустимой длины.
	/// </summary>
	[TestMethod]
	public async Task Handle_WhenDetailsHaveMaximumLength_UpdatesCategory()
	{
		await using var context = CreateContext();
		var category = await AddCategory(context);
		var handler = CreateHandler(context);
		var name = new string('N', CategoryConstraints.MaxNameLength);
		var description = new string('D', CategoryConstraints.MaxDescriptionLength);

		var result = await handler.Handle(
			new(category.Id, name, description), CancellationToken.None);

		Assert.IsTrue(result.IsSuccess);
		context.ChangeTracker.Clear();
		var savedCategory = await context.Categories.SingleAsync();
		Assert.AreEqual(name, savedCategory.Name);
		Assert.AreEqual(description, savedCategory.Description);
	}

	/// <summary>
	/// Проверяет возврат ошибки отсутствия организации у текущего пользователя.
	/// </summary>
	[TestMethod]
	public async Task Handle_WhenOrganizationIsMissing_ReturnsOrganizationNotFound()
	{
		await using var context = CreateContext();
		CurrentOrganization.OrganizationId.Returns((int?)null);
		var handler = CreateHandler(context);

		var result = await handler.Handle(new(1, "Updated", null), CancellationToken.None);

		Assert.IsTrue(result.IsFailure);
		Assert.AreEqual(CategoryErrors.OrganizationNotFound, result.Error);
	}

	/// <summary>
	/// Проверяет отказ в обновлении и сохранность категории при отсутствии разрешения Categories.Update.
	/// </summary>
	[TestMethod]
	public async Task Handle_WhenUpdatePermissionIsMissing_DoesNotChangeCategory()
	{
		await using var context = CreateContext();
		var category = await AddCategory(context);
		PermissionChecker
			.HasPermission(Permissions.Categories.Update, Arg.Any<CancellationToken>())
			.Returns(Task.FromResult(false));
		var handler = CreateHandler(context);

		var result = await handler.Handle(
			new(category.Id, "Updated", null), CancellationToken.None);

		Assert.IsTrue(result.IsFailure);
		Assert.AreEqual(CategoryErrors.Forbidden, result.Error);
		await PermissionChecker.Received().HasPermission(
			Permissions.Categories.Update, CancellationToken.None);
		await AssertCategoryUnchanged(context);
	}

	/// <summary>
	/// Проверяет возврат ошибки поиска без создания записи, если категория не существует.
	/// </summary>
	[TestMethod]
	public async Task Handle_WhenCategoryDoesNotExist_ReturnsCategoryNotFound()
	{
		await using var context = CreateContext();
		var handler = CreateHandler(context);

		var result = await handler.Handle(new(999, "Updated", null), CancellationToken.None);

		Assert.IsTrue(result.IsFailure);
		Assert.AreEqual(CategoryErrors.CategoryNotFound, result.Error);
		Assert.AreEqual(0, await context.Categories.CountAsync());
	}

	/// <summary>
	/// Проверяет возврат ошибки поиска и сохранность категории другой организации.
	/// </summary>
	[TestMethod]
	public async Task Handle_WhenCategoryBelongsToAnotherOrganization_ReturnsCategoryNotFound()
	{
		await using var context = CreateContext();
		var category = await AddCategory(context, organizationId: OrganizationId + 1);
		var handler = CreateHandler(context);

		var result = await handler.Handle(
			new(category.Id, "Updated", null), CancellationToken.None);

		Assert.IsTrue(result.IsFailure);
		Assert.AreEqual(CategoryErrors.CategoryNotFound, result.Error);
		await AssertCategoryUnchanged(context);
	}

	/// <summary>
	/// Проверяет запрет изменения глобальной категории и сохранность ее данных.
	/// </summary>
	[TestMethod]
	public async Task Handle_WhenCategoryIsGlobal_ReturnsReadOnlyError()
	{
		await using var context = CreateContext();
		var category = await AddCategory(context, organizationId: null);
		var handler = CreateHandler(context);

		var result = await handler.Handle(
			new(category.Id, "Updated", null), CancellationToken.None);

		Assert.IsTrue(result.IsFailure);
		Assert.AreEqual(CategoryErrors.GlobalCategoryReadOnly, result.Error);
		await AssertCategoryUnchanged(context);
	}

	/// <summary>
	/// Проверяет ошибку валидации и сохранность категории при пустом или пробельном названии.
	/// </summary>
	/// <param name="name"> Некорректное название категории. </param>
	[TestMethod]
	[DataRow(null)]
	[DataRow("")]
	[DataRow("   ")]
	public async Task Handle_WhenNameIsEmpty_ReturnsValidationError(string? name)
	{
		await using var context = CreateContext();
		var category = await AddCategory(context);
		var handler = CreateHandler(context);

		var result = await handler.Handle(
			new(category.Id, name!, "Updated description"), CancellationToken.None);

		Assert.IsTrue(result.IsFailure);
		Assert.AreEqual("Category.Validation", result.Error.Code);
		Assert.AreEqual(ErrorType.Validation, result.Error.Type);
		await AssertCategoryUnchanged(context);
	}

	/// <summary>
	/// Проверяет ошибку валидации и сохранность категории при превышении допустимой длины данных.
	/// </summary>
	/// <param name="nameLength"> Длина названия категории. </param>
	/// <param name="descriptionLength"> Длина описания категории. </param>
	[TestMethod]
	[DataRow(CategoryConstraints.MaxNameLength + 1, 1)]
	[DataRow(1, CategoryConstraints.MaxDescriptionLength + 1)]
	public async Task Handle_WhenDetailsAreTooLong_ReturnsValidationError(int nameLength, int descriptionLength)
	{
		await using var context = CreateContext();
		var category = await AddCategory(context);
		var handler = CreateHandler(context);

		var result = await handler.Handle(
			new(category.Id, new string('N', nameLength), new string('D', descriptionLength)),
			CancellationToken.None);

		Assert.IsTrue(result.IsFailure);
		Assert.AreEqual("Category.Validation", result.Error.Code);
		Assert.AreEqual(ErrorType.Validation, result.Error.Type);
		await AssertCategoryUnchanged(context);
	}

	/// <summary>
	/// Проверяет ошибку валидации при неположительном идентификаторе категории.
	/// </summary>
	/// <param name="categoryId"> Некорректный идентификатор категории. </param>
	[TestMethod]
	[DataRow(0)]
	[DataRow(-1)]
	public async Task Handle_WhenCategoryIdIsNotPositive_ReturnsValidationError(int categoryId)
	{
		await using var context = CreateContext();
		var handler = CreateHandler(context);

		var result = await handler.Handle(new(categoryId, "Updated", null), CancellationToken.None);

		Assert.IsTrue(result.IsFailure);
		Assert.AreEqual("Category.Validation", result.Error.Code);
		Assert.AreEqual(ErrorType.Validation, result.Error.Type);
	}

	/// <summary>
	/// Создает контекст с отдельной InMemory-базой для изоляции теста.
	/// </summary>
	/// <returns> Контекст тестовой базы данных. </returns>
	private static ApplicationDbContext CreateContext()
	{
		var options = new DbContextOptionsBuilder<ApplicationDbContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;

		return new ApplicationDbContext(options);
	}

	/// <summary>
	/// Сохраняет исходную категорию с заполненным аудитом создания.
	/// </summary>
	/// <param name="context"> Контекст тестовой базы данных. </param>
	/// <param name="organizationId"> Организация категории; null создает глобальную категорию. </param>
	/// <param name="parentId"> Идентификатор родительской категории. </param>
	/// <returns> Сохраненную категорию. </returns>
	private static async Task<Category> AddCategory(
		ApplicationDbContext context,
		int? organizationId = OrganizationId,
		int? parentId = null)
	{
		var category = new Category(
			"Original", CategoryType.Expense, ActivityType.Operating,
			"Original description", organizationId, parentId);
		category.MarkCreated(CreatorId);
		context.Categories.Add(category);
		await context.SaveChangesAsync();

		return category;
	}

	/// <summary>
	/// Создает обработчик с подменами контекстов и прав и настоящим валидатором команды.
	/// </summary>
	/// <param name="context"> Контекст тестовой базы данных. </param>
	/// <returns> Обработчик обновления категории. </returns>
	private UpdateCategoryHandler CreateHandler(ApplicationDbContext context)
	{
		return new UpdateCategoryHandler(
			context,
			CurrentUser,
			CurrentOrganization,
			PermissionChecker,
			new UpdateCategoryCommandValidator());
	}

	/// <summary>
	/// Проверяет, что данные категории и автор последнего изменения остались прежними в базе.
	/// </summary>
	/// <param name="context"> Контекст тестовой базы данных. </param>
	private static async Task AssertCategoryUnchanged(ApplicationDbContext context)
	{
		context.ChangeTracker.Clear();
		var category = await context.Categories.SingleAsync();
		Assert.AreEqual("Original", category.Name);
		Assert.AreEqual("Original description", category.Description);
		Assert.AreEqual(CreatorId, category.UpdatedBy);
	}
}
