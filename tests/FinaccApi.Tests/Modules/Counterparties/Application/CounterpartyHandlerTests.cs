using FinaccApi.DataAccess;
using FinaccApi.Modules.Access.Domain;
using FinaccApi.Modules.Access.Infrastructure;
using FinaccApi.Modules.Counterparties;
using FinaccApi.Modules.Counterparties.Api;
using FinaccApi.Modules.Counterparties.Application;
using FinaccApi.Modules.Counterparties.Application.ArchiveCounterparty;
using FinaccApi.Modules.Counterparties.Application.CreateCounterparty;
using FinaccApi.Modules.Counterparties.Application.GetCounterparties;
using FinaccApi.Modules.Counterparties.Application.GetCounterpartyById;
using FinaccApi.Modules.Counterparties.Application.UpdateCounterparty;
using FinaccApi.Modules.Counterparties.Domain;
using FinaccApi.Shared.Application.Auth;
using FinaccApi.Shared.Application.MultiTenancy;
using FinaccApi.Shared.Kernel;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace FinaccApi.Tests.Modules.Counterparties.Application;

/// <summary>
/// Проверки сценариев контрагентов с настоящими валидаторами и изолированной InMemory-базой.
/// </summary>
[TestClass]
public sealed class CounterpartyHandlerTests
{
	private const int OrganizationId = 7;
	private const int CreatorId = 22;
	private const int CurrentUserId = 11;
	private ICurrentUser _currentUser = null!;
	private ICurrentOrganization _currentOrganization = null!;
	private IPermissionChecker _permissionChecker = null!;

	/// <summary>
	/// Создает контексты, требующие загрузки организации до проверки разрешений.
	/// </summary>
	[TestInitialize]
	public void Initialize()
	{
		_currentUser = Substitute.For<ICurrentUser>();
		_currentUser.RequiredUserId.Returns(CurrentUserId);
		_currentUser.UserId.Returns(CurrentUserId);
		_currentUser.IsAuthenticated.Returns(true);
		_currentOrganization = Substitute.For<ICurrentOrganization>();
		_currentOrganization.OrganizationId.Returns((int?)OrganizationId);
		_currentOrganization.MembershipId.Returns((int?)70);

		var contextLoaded = false;
		_currentOrganization.LoadContext(Arg.Any<CancellationToken>()).Returns(call =>
		{
			call.Arg<CancellationToken>().ThrowIfCancellationRequested();
			contextLoaded = true;
			return Task.CompletedTask;
		});
		_currentOrganization.HasOrganization.Returns(_ => contextLoaded
			? _currentOrganization.OrganizationId.HasValue
			: throw new InvalidOperationException("Organization context is not loaded."));

		_permissionChecker = Substitute.For<IPermissionChecker>();
		_permissionChecker.HasPermission(Arg.Any<string>(), Arg.Any<CancellationToken>())
			.Returns(Task.FromResult(true));
	}

	/// <summary>
	/// Проверяет сохранение данных, организации и полного аудита создания.
	/// </summary>
	[TestMethod]
	public async Task Create_WhenValid_SavesDetailsOrganizationAndAudit()
	{
		await using var context = CreateContext();
		var beforeCreation = DateTime.UtcNow;
		var result = await CreateHandler(context).Handle(ValidCreateCommand(), CancellationToken.None);

		Assert.IsTrue(result.IsSuccess);
		Assert.IsTrue(result.Value.Id > 0);
		Assert.AreEqual(OrganizationId, result.Value.OrganizationId);
		Assert.AreEqual("Original", result.Value.Name);
		Assert.AreEqual(CounterpartyType.LegalEntity, result.Value.Type);
		Assert.AreEqual("Поставщик", result.Value.Category);
		Assert.AreEqual("+7 900 123-45-67", result.Value.Phone);
		Assert.AreEqual("contact@example.com", result.Value.Email);
		Assert.IsFalse(result.Value.IsArchived);
		context.ChangeTracker.Clear();
		var saved = await context.Counterparties.SingleAsync();
		Assert.AreEqual(OrganizationId, saved.OrganizationId);
		Assert.AreEqual(CurrentUserId, saved.CreatedBy);
		Assert.AreEqual(CurrentUserId, saved.UpdatedBy);
		Assert.IsTrue(saved.CreatedAt >= beforeCreation && saved.CreatedAt <= DateTime.UtcNow);
		Assert.IsTrue(saved.UpdatedAt >= beforeCreation && saved.UpdatedAt <= DateTime.UtcNow);
		Assert.AreEqual(DateTimeKind.Utc, saved.CreatedAt.Kind);
		Assert.AreEqual(DateTimeKind.Utc, saved.UpdatedAt.Kind);
		await _permissionChecker.Received(1).HasPermission(Permissions.Counterparties.Create, CancellationToken.None);
	}

	/// <summary>
	/// Проверяет необязательные данные и допустимость одинаковых имен в одной организации.
	/// </summary>
	[TestMethod]
	public async Task Create_WhenOptionalFieldsAreNullAndNameIsDuplicated_CreatesBothRecords()
	{
		await using var context = CreateContext();
		var handler = CreateHandler(context);
		var command = new CreateCounterpartyCommand("Same name", null, null, null, null);

		var first = await handler.Handle(command, CancellationToken.None);
		var second = await handler.Handle(command, CancellationToken.None);

		Assert.IsTrue(first.IsSuccess);
		Assert.IsTrue(second.IsSuccess);
		Assert.AreNotEqual(first.Value.Id, second.Value.Id);
		Assert.IsNull(second.Value.Type);
		Assert.IsNull(second.Value.Category);
		Assert.IsNull(second.Value.Phone);
		Assert.IsNull(second.Value.Email);
		Assert.AreEqual(2, await context.Counterparties.CountAsync());
	}

	/// <summary>
	/// Проверяет граничные допустимые длины без зависимости от ограничений PostgreSQL.
	/// </summary>
	[TestMethod]
	public async Task Create_WhenFieldsHaveMaximumLengths_Succeeds()
	{
		await using var context = CreateContext();
		var command = new CreateCounterpartyCommand(
			new string('N', CounterpartyConstraints.MaxNameLength),
			CounterpartyType.Individual,
			new string('C', CounterpartyConstraints.MaxCategoryLength),
			new string('1', CounterpartyConstraints.MaxPhoneLength),
			new string('e', CounterpartyConstraints.MaxEmailLength - "@example.com".Length) + "@example.com");

		var result = await CreateHandler(context).Handle(command, CancellationToken.None);

		Assert.IsTrue(result.IsSuccess);
		Assert.AreEqual(command.Name, result.Value.Name);
		Assert.AreEqual(command.Category, result.Value.Category);
		Assert.AreEqual(command.Phone, result.Value.Phone);
		Assert.AreEqual(command.Email, result.Value.Email);
	}

	/// <summary>
	/// Проверяет, что некорректные данные создания возвращают Result и не сохраняются.
	/// </summary>
	[TestMethod]
	[DataRow("NullName")]
	[DataRow("EmptyName")]
	[DataRow("WhitespaceName")]
	[DataRow("LongName")]
	[DataRow("LongCategory")]
	[DataRow("LongPhone")]
	[DataRow("LongEmail")]
	[DataRow("InvalidEmail")]
	[DataRow("InvalidType")]
	public async Task Create_WhenDetailsAreInvalid_ReturnsValidationWithoutSaving(string invalidField)
	{
		await using var context = CreateContext();

		var result = await CreateHandler(context).Handle(InvalidCommand(invalidField), CancellationToken.None);

		AssertValidation(result);
		Assert.AreEqual(0, await context.Counterparties.CountAsync());
	}

	/// <summary>
	/// Проверяет загрузку контекста и отсутствие бизнес-операций при отсутствии организации.
	/// </summary>
	[TestMethod]
	[DataRow(0)]
	[DataRow(1)]
	[DataRow(2)]
	[DataRow(3)]
	[DataRow(4)]
	public async Task Handle_WhenOrganizationIsMissing_ReturnsOrganizationNotFound(int operation)
	{
		await using var context = CreateContext();
		var counterparty = await AddCounterparty(context);
		_currentOrganization.OrganizationId.Returns((int?)null);

		var result = await InvokeOperation(operation, context, counterparty.Id);

		Assert.IsTrue(result.IsFailure);
		Assert.AreEqual(CounterpartyErrors.OrganizationNotFound, result.Error);
		await _currentOrganization.Received(1).LoadContext(CancellationToken.None);
		await _permissionChecker.DidNotReceive().HasPermission(Arg.Any<string>(), Arg.Any<CancellationToken>());
		await AssertOriginalRecord(context);
	}

	/// <summary>
	/// Проверяет собственное разрешение каждого сценария и сохранность записи при отказе.
	/// </summary>
	[TestMethod]
	[DataRow(0, Permissions.Counterparties.Create)]
	[DataRow(1, Permissions.Counterparties.View)]
	[DataRow(2, Permissions.Counterparties.View)]
	[DataRow(3, Permissions.Counterparties.Update)]
	[DataRow(4, Permissions.Counterparties.Archive)]
	public async Task Handle_WhenPermissionIsMissing_ReturnsForbiddenWithoutChanges(int operation, string permission)
	{
		await using var context = CreateContext();
		var counterparty = await AddCounterparty(context);
		_permissionChecker.HasPermission(permission, Arg.Any<CancellationToken>()).Returns(Task.FromResult(false));

		var result = await InvokeOperation(operation, context, counterparty.Id);

		Assert.IsTrue(result.IsFailure);
		Assert.AreEqual(CounterpartyErrors.Forbidden, result.Error);
		await _permissionChecker.Received(1).HasPermission(permission, CancellationToken.None);
		await AssertOriginalRecord(context);
	}

	/// <summary>
	/// Проверяет реальный контекст Access: неактивное членство не разрешает ни один сценарий.
	/// </summary>
	[TestMethod]
	[DataRow("removed")]
	[DataRow("left")]
	[DataRow("suspended")]
	public async Task Handle_WhenMembershipIsInactive_ReturnsOrganizationNotFound(string membershipStatus)
	{
		await using var context = CreateContext();
		var counterparty = await AddCounterparty(context);
		var membership = new OrganizationMember(CurrentUserId, OrganizationId, 1, membershipStatus);
		membership.MarkCreated(CreatorId);
		context.OrganizationMembers.Add(membership);
		await context.SaveChangesAsync();

		for (var operation = 0; operation < 5; operation++)
		{
			var organization = new CurrentOrganization(context, _currentUser);
			var result = await InvokeOperation(operation, context, counterparty.Id, organization, new PermissionChecker(organization));
			Assert.IsTrue(result.IsFailure, $"Операция {operation} разрешена неактивному участнику.");
			Assert.AreEqual(CounterpartyErrors.OrganizationNotFound, result.Error);
		}

		await AssertOriginalRecord(context);
	}

	/// <summary>
	/// Проверяет фильтрацию организации и архива, сортировку по имени и ID и отсутствие tracking.
	/// </summary>
	[TestMethod]
	public async Task GetList_ByDefault_ReturnsOnlyActiveOwnRecordsInStableOrder()
	{
		await using var context = CreateContext();
		var beta = await AddCounterparty(context, name: "Beta");
		var alphaFirst = await AddCounterparty(context, name: "Alpha");
		var alphaSecond = await AddCounterparty(context, name: "Alpha");
		await AddCounterparty(context, name: "Archived", archived: true);
		await AddCounterparty(context, organizationId: OrganizationId + 1, name: "Foreign");
		context.ChangeTracker.Clear();

		var result = await ListHandler(context).Handle(new(), CancellationToken.None);

		Assert.IsTrue(result.IsSuccess);
		CollectionAssert.AreEqual(new[] { alphaFirst.Id, alphaSecond.Id, beta.Id }, result.Value.Select(x => x.Id).ToArray());
		Assert.IsTrue(result.Value.All(x => x.OrganizationId == OrganizationId && !x.IsArchived));
		Assert.AreEqual(0, context.ChangeTracker.Entries<Counterparty>().Count());
		await _permissionChecker.Received(1).HasPermission(Permissions.Counterparties.View, CancellationToken.None);
	}

	/// <summary>
	/// Проверяет включение архивных записей с сохранением изоляции организаций.
	/// </summary>
	[TestMethod]
	public async Task GetList_WhenIncludingArchived_ReturnsOwnActiveAndArchivedRecords()
	{
		await using var context = CreateContext();
		var active = await AddCounterparty(context);
		var archived = await AddCounterparty(context, archived: true);
		await AddCounterparty(context, organizationId: OrganizationId + 1, archived: true);

		var result = await ListHandler(context).Handle(new(IncludeArchived: true), CancellationToken.None);

		Assert.IsTrue(result.IsSuccess);
		CollectionAssert.AreEquivalent(new[] { active.Id, archived.Id }, result.Value.Select(x => x.Id).ToArray());
		Assert.IsTrue(result.Value.Single(x => x.Id == archived.Id).IsArchived);
	}

	/// <summary>
	/// Проверяет фильтр типа без выборки null, других типов и записей другой организации.
	/// </summary>
	[TestMethod]
	public async Task GetList_WhenTypeIsSpecified_ReturnsOnlyMatchingOwnRecords()
	{
		await using var context = CreateContext();
		var individual = await AddCounterparty(context, type: CounterpartyType.Individual);
		await AddCounterparty(context, type: CounterpartyType.LegalEntity);
		await AddCounterparty(context, type: null);
		await AddCounterparty(context, organizationId: OrganizationId + 1, type: CounterpartyType.Individual);

		var result = await ListHandler(context).Handle(new(Type: CounterpartyType.Individual), CancellationToken.None);

		Assert.IsTrue(result.IsSuccess);
		Assert.AreEqual(individual.Id, result.Value.Single().Id);
	}

	/// <summary>
	/// Проверяет Result-валидацию неизвестного типа фильтра вместо исключения.
	/// </summary>
	[TestMethod]
	public async Task GetList_WhenTypeIsUnknown_ReturnsValidation()
	{
		await using var context = CreateContext();

		var result = await ListHandler(context).Handle(new(Type: (CounterpartyType)99), CancellationToken.None);

		AssertValidation(result);
	}

	/// <summary>
	/// Проверяет доступность активной и архивной записи по ID с полным DTO и без tracking.
	/// </summary>
	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public async Task GetById_WhenRecordIsOwned_ReturnsDetailsIncludingArchived(bool archived)
	{
		await using var context = CreateContext();
		var counterparty = await AddCounterparty(context, archived: archived);
		context.ChangeTracker.Clear();

		var result = await ByIdHandler(context).Handle(new(counterparty.Id), CancellationToken.None);

		Assert.IsTrue(result.IsSuccess);
		Assert.AreEqual(counterparty.Id, result.Value.Id);
		Assert.AreEqual(OrganizationId, result.Value.OrganizationId);
		Assert.AreEqual("Original", result.Value.Name);
		Assert.AreEqual(CounterpartyType.LegalEntity, result.Value.Type);
		Assert.AreEqual("Поставщик", result.Value.Category);
		Assert.AreEqual("+7 900 123-45-67", result.Value.Phone);
		Assert.AreEqual("contact@example.com", result.Value.Email);
		Assert.AreEqual(archived, result.Value.IsArchived);
		Assert.AreEqual(0, context.ChangeTracker.Entries<Counterparty>().Count());
	}

	/// <summary>
	/// Проверяет одинаковый NotFound для чужих и отсутствующих ID при чтении, изменении и архивировании.
	/// </summary>
	[TestMethod]
	[DataRow(2, false)]
	[DataRow(3, false)]
	[DataRow(4, false)]
	[DataRow(2, true)]
	[DataRow(3, true)]
	[DataRow(4, true)]
	public async Task Handle_WhenRecordIsForeignOrMissing_ReturnsNotFound(int operation, bool missing)
	{
		await using var context = CreateContext();
		var foreign = await AddCounterparty(context, organizationId: OrganizationId + 1);

		var result = await InvokeOperation(operation, context, missing ? foreign.Id + 100 : foreign.Id);

		Assert.IsTrue(result.IsFailure);
		Assert.AreEqual(CounterpartyErrors.NotFound, result.Error);
		await AssertOriginalRecord(context, OrganizationId + 1);
	}

	/// <summary>
	/// Проверяет полную замену данных, сохранение автора создания и обновление аудита.
	/// </summary>
	[TestMethod]
	public async Task Update_WhenValid_ReplacesDetailsAndUpdatesAudit()
	{
		await using var context = CreateContext();
		var counterparty = await AddCounterparty(context);
		var createdAt = counterparty.CreatedAt;
		var beforeUpdate = DateTime.UtcNow;

		var result = await UpdateHandler(context).Handle(
			new(counterparty.Id, "Updated", CounterpartyType.Individual, "Покупатель", "123", "new@example.com"),
			CancellationToken.None);

		Assert.IsTrue(result.IsSuccess);
		Assert.AreEqual(counterparty.Id, result.Value.Id);
		Assert.AreEqual(OrganizationId, result.Value.OrganizationId);
		Assert.AreEqual("Updated", result.Value.Name);
		Assert.AreEqual(CounterpartyType.Individual, result.Value.Type);
		Assert.AreEqual("Покупатель", result.Value.Category);
		Assert.AreEqual("123", result.Value.Phone);
		Assert.AreEqual("new@example.com", result.Value.Email);
		context.ChangeTracker.Clear();
		var saved = await context.Counterparties.SingleAsync();
		Assert.AreEqual("Updated", saved.Name);
		Assert.AreEqual(CreatorId, saved.CreatedBy);
		Assert.AreEqual(createdAt, saved.CreatedAt);
		Assert.AreEqual(CurrentUserId, saved.UpdatedBy);
		Assert.IsTrue(saved.UpdatedAt >= beforeUpdate && saved.UpdatedAt <= DateTime.UtcNow);
		await _permissionChecker.Received(1).HasPermission(Permissions.Counterparties.Update, CancellationToken.None);
	}

	/// <summary>
	/// Проверяет семантику PUT: null очищает все необязательные поля.
	/// </summary>
	[TestMethod]
	public async Task Update_WhenOptionalFieldsAreNull_ClearsAllOptionalFields()
	{
		await using var context = CreateContext();
		var counterparty = await AddCounterparty(context);

		var result = await UpdateHandler(context).Handle(new(counterparty.Id, "Updated", null, null, null, null), CancellationToken.None);

		Assert.IsTrue(result.IsSuccess);
		Assert.IsNull(result.Value.Type);
		Assert.IsNull(result.Value.Category);
		Assert.IsNull(result.Value.Phone);
		Assert.IsNull(result.Value.Email);
		context.ChangeTracker.Clear();
		var saved = await context.Counterparties.SingleAsync();
		Assert.IsNull(saved.Type);
		Assert.IsNull(saved.Category);
		Assert.IsNull(saved.Phone);
		Assert.IsNull(saved.Email);
	}

	/// <summary>
	/// Проверяет сохранность полей и аудита при ошибке валидации обновления.
	/// </summary>
	[TestMethod]
	[DataRow("NullName")]
	[DataRow("EmptyName")]
	[DataRow("WhitespaceName")]
	[DataRow("LongName")]
	[DataRow("LongCategory")]
	[DataRow("LongPhone")]
	[DataRow("LongEmail")]
	[DataRow("InvalidEmail")]
	[DataRow("InvalidType")]
	public async Task Update_WhenDetailsAreInvalid_DoesNotChangeFieldsOrAudit(string invalidField)
	{
		await using var context = CreateContext();
		var counterparty = await AddCounterparty(context);
		var updatedAt = counterparty.UpdatedAt;
		var details = InvalidCommand(invalidField);

		var result = await UpdateHandler(context).Handle(
			new(counterparty.Id, details.Name, details.Type, details.Category, details.Phone, details.Email),
			CancellationToken.None);

		AssertValidation(result);
		var saved = await AssertOriginalRecord(context);
		Assert.AreEqual(updatedAt, saved.UpdatedAt);
	}

	/// <summary>
	/// Проверяет запрет редактирования архива с Conflict и сохранением данных.
	/// </summary>
	[TestMethod]
	public async Task Update_WhenArchived_ReturnsConflictWithoutChanges()
	{
		await using var context = CreateContext();
		var counterparty = await AddCounterparty(context, archived: true);
		var updatedAt = counterparty.UpdatedAt;

		var result = await UpdateHandler(context).Handle(new(counterparty.Id, "Updated", null, null, null, null), CancellationToken.None);

		Assert.IsTrue(result.IsFailure);
		Assert.AreEqual(CounterpartyErrors.Archived, result.Error);
		Assert.AreEqual(ErrorType.Conflict, result.Error.Type);
		var saved = await AssertOriginalRecord(context, archived: true);
		Assert.AreEqual(updatedAt, saved.UpdatedAt);
	}

	/// <summary>
	/// Проверяет валидацию маршрутного ID до доступа к контрагенту.
	/// </summary>
	[TestMethod]
	[DataRow(2, 0)]
	[DataRow(2, -1)]
	[DataRow(3, 0)]
	[DataRow(3, -1)]
	[DataRow(4, 0)]
	[DataRow(4, -1)]
	public async Task Handle_WhenRouteIdIsNotPositive_ReturnsValidation(int operation, int id)
	{
		await using var context = CreateContext();

		var result = await InvokeOperation(operation, context, id);

		AssertValidation(result);
		Assert.AreEqual(0, await context.Counterparties.CountAsync());
	}

	/// <summary>
	/// Проверяет архивирование без удаления записи и с сохранением аудита создания.
	/// </summary>
	[TestMethod]
	public async Task Archive_WhenActive_SetsFlagAndAuditWithoutDeleting()
	{
		await using var context = CreateContext();
		var counterparty = await AddCounterparty(context);
		var createdAt = counterparty.CreatedAt;
		var beforeArchive = DateTime.UtcNow;

		var result = await ArchiveHandler(context).Handle(new(counterparty.Id), CancellationToken.None);

		Assert.IsTrue(result.IsSuccess);
		context.ChangeTracker.Clear();
		var saved = await context.Counterparties.SingleAsync();
		Assert.IsTrue(saved.IsArchived);
		Assert.AreEqual("Original", saved.Name);
		Assert.AreEqual(CreatorId, saved.CreatedBy);
		Assert.AreEqual(createdAt, saved.CreatedAt);
		Assert.AreEqual(CurrentUserId, saved.UpdatedBy);
		Assert.IsTrue(saved.UpdatedAt >= beforeArchive && saved.UpdatedAt <= DateTime.UtcNow);
		await _permissionChecker.Received(1).HasPermission(Permissions.Counterparties.Archive, CancellationToken.None);
	}

	/// <summary>
	/// Проверяет идемпотентность архивирования, включая неизменность автора и времени последнего изменения.
	/// </summary>
	[TestMethod]
	public async Task Archive_WhenAlreadyArchived_DoesNotChangeAudit()
	{
		await using var context = CreateContext();
		var counterparty = await AddCounterparty(context, archived: true);
		var createdAt = counterparty.CreatedAt;
		var updatedAt = counterparty.UpdatedAt;
		var handler = ArchiveHandler(context);

		var first = await handler.Handle(new(counterparty.Id), CancellationToken.None);
		var second = await handler.Handle(new(counterparty.Id), CancellationToken.None);

		Assert.IsTrue(first.IsSuccess);
		Assert.IsTrue(second.IsSuccess);
		var saved = await AssertOriginalRecord(context, archived: true);
		Assert.AreEqual(createdAt, saved.CreatedAt);
		Assert.AreEqual(updatedAt, saved.UpdatedAt);
		Assert.AreEqual(CreatorId, saved.UpdatedBy);
	}

	/// <summary>
	/// Проверяет регистрацию всех обработчиков и валидаторов и построение контроллера из DI.
	/// </summary>
	[TestMethod]
	public async Task Module_WhenRegistered_ResolvesHandlersValidatorsAndController()
	{
		var services = new ServiceCollection();
		services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
		services.AddScoped<ICurrentUser>(_ => _currentUser);
		services.AddScoped<ICurrentOrganization>(_ => _currentOrganization);
		services.AddScoped<IPermissionChecker>(_ => _permissionChecker);
		services.AddCounterpartiesModule(new ConfigurationBuilder().Build());
		await using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
		await using var scope = provider.CreateAsyncScope();

		Assert.IsNotNull(scope.ServiceProvider.GetRequiredService<CreateCounterpartyHandler>());
		Assert.IsNotNull(scope.ServiceProvider.GetRequiredService<GetCounterpartiesHandler>());
		Assert.IsNotNull(scope.ServiceProvider.GetRequiredService<GetCounterpartyByIdHandler>());
		Assert.IsNotNull(scope.ServiceProvider.GetRequiredService<UpdateCounterpartyHandler>());
		Assert.IsNotNull(scope.ServiceProvider.GetRequiredService<ArchiveCounterpartyHandler>());
		Assert.IsInstanceOfType<CreateCounterpartyCommandValidator>(scope.ServiceProvider.GetRequiredService<IValidator<CreateCounterpartyCommand>>());
		Assert.IsInstanceOfType<UpdateCounterpartyCommandValidator>(scope.ServiceProvider.GetRequiredService<IValidator<UpdateCounterpartyCommand>>());
		Assert.IsInstanceOfType<ArchiveCounterpartyCommandValidator>(scope.ServiceProvider.GetRequiredService<IValidator<ArchiveCounterpartyCommand>>());
		Assert.IsNotNull(ActivatorUtilities.CreateInstance<CounterpartiesController>(scope.ServiceProvider));
	}

	private static ApplicationDbContext CreateContext()
		=> new(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

	private CreateCounterpartyHandler CreateHandler(ApplicationDbContext context)
		=> new(context, _currentUser, _currentOrganization, _permissionChecker, new CreateCounterpartyCommandValidator());

	private UpdateCounterpartyHandler UpdateHandler(ApplicationDbContext context)
		=> new(context, _currentUser, _currentOrganization, _permissionChecker, new UpdateCounterpartyCommandValidator());

	private ArchiveCounterpartyHandler ArchiveHandler(ApplicationDbContext context)
		=> new(context, _currentUser, _currentOrganization, _permissionChecker, new ArchiveCounterpartyCommandValidator());

	private GetCounterpartiesHandler ListHandler(ApplicationDbContext context)
		=> new(context, _currentOrganization, _permissionChecker);

	private GetCounterpartyByIdHandler ByIdHandler(ApplicationDbContext context)
		=> new(context, _currentOrganization, _permissionChecker);

	private async Task<Result> InvokeOperation(
		int operation, ApplicationDbContext context, int id,
		ICurrentOrganization? organization = null, IPermissionChecker? permissionChecker = null)
	{
		organization ??= _currentOrganization;
		permissionChecker ??= _permissionChecker;
		return operation switch
		{
			0 => await new CreateCounterpartyHandler(context, _currentUser, organization, permissionChecker, new CreateCounterpartyCommandValidator())
				.Handle(ValidCreateCommand(), CancellationToken.None),
			1 => await new GetCounterpartiesHandler(context, organization, permissionChecker)
				.Handle(new(), CancellationToken.None),
			2 => await new GetCounterpartyByIdHandler(context, organization, permissionChecker)
				.Handle(new(id), CancellationToken.None),
			3 => await new UpdateCounterpartyHandler(context, _currentUser, organization, permissionChecker, new UpdateCounterpartyCommandValidator())
				.Handle(new(id, "Updated", null, null, null, null), CancellationToken.None),
			4 => await new ArchiveCounterpartyHandler(context, _currentUser, organization, permissionChecker, new ArchiveCounterpartyCommandValidator())
				.Handle(new(id), CancellationToken.None),
			_ => throw new ArgumentOutOfRangeException(nameof(operation))
		};
	}

	private static CreateCounterpartyCommand ValidCreateCommand()
		=> new("Original", CounterpartyType.LegalEntity, "Поставщик", "+7 900 123-45-67", "contact@example.com");

	private static CreateCounterpartyCommand InvalidCommand(string invalidField)
	{
		var valid = ValidCreateCommand();
		return invalidField switch
		{
			"NullName" => valid with { Name = null! },
			"EmptyName" => valid with { Name = "" },
			"WhitespaceName" => valid with { Name = " \t\r\n " },
			"LongName" => valid with { Name = new string('N', CounterpartyConstraints.MaxNameLength + 1) },
			"LongCategory" => valid with { Category = new string('C', CounterpartyConstraints.MaxCategoryLength + 1) },
			"LongPhone" => valid with { Phone = new string('1', CounterpartyConstraints.MaxPhoneLength + 1) },
			"LongEmail" => valid with { Email = new string('e', CounterpartyConstraints.MaxEmailLength) + "@example.com" },
			"InvalidEmail" => valid with { Email = "not-an-email" },
			"InvalidType" => valid with { Type = (CounterpartyType)99 },
			_ => throw new ArgumentOutOfRangeException(nameof(invalidField))
		};
	}

	private static async Task<Counterparty> AddCounterparty(
		ApplicationDbContext context, int organizationId = OrganizationId, string name = "Original",
		CounterpartyType? type = CounterpartyType.LegalEntity, bool archived = false)
	{
		var counterparty = new Counterparty(organizationId, name, type, "Поставщик", "+7 900 123-45-67", "contact@example.com");
		counterparty.MarkCreated(CreatorId);
		if (archived)
			counterparty.Archive();
		context.Counterparties.Add(counterparty);
		await context.SaveChangesAsync();
		return counterparty;
	}

	private static async Task<Counterparty> AssertOriginalRecord(ApplicationDbContext context, int organizationId = OrganizationId, bool archived = false)
	{
		context.ChangeTracker.Clear();
		var saved = await context.Counterparties.SingleAsync();
		Assert.AreEqual(organizationId, saved.OrganizationId);
		Assert.AreEqual("Original", saved.Name);
		Assert.AreEqual(CounterpartyType.LegalEntity, saved.Type);
		Assert.AreEqual("Поставщик", saved.Category);
		Assert.AreEqual("+7 900 123-45-67", saved.Phone);
		Assert.AreEqual("contact@example.com", saved.Email);
		Assert.AreEqual(archived, saved.IsArchived);
		Assert.AreEqual(CreatorId, saved.CreatedBy);
		Assert.AreEqual(CreatorId, saved.UpdatedBy);
		return saved;
	}

	private static void AssertValidation(Result result)
	{
		Assert.IsTrue(result.IsFailure);
		Assert.AreEqual("Counterparty.Validation", result.Error.Code);
		Assert.AreEqual(ErrorType.Validation, result.Error.Type);
	}
}
