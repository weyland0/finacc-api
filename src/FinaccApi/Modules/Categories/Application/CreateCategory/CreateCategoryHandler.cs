using Microsoft.EntityFrameworkCore;
using FinaccApi.DataAccess;
using FinaccApi.Shared.Kernel;
using FinaccApi.Shared.Application.Auth;
using FinaccApi.Shared.Application.MultiTenancy;
using FinaccApi.Modules.Access.Domain;
using FinaccApi.Modules.Categories.Domain;
using FluentValidation;

namespace FinaccApi.Modules.Categories.Application.CreateCategory;

/// <summary>
/// Обработчик создания категории.
/// </summary>
public class CreateCategoryHandler
{
	/// <summary>
	/// Контекст базы данных.
	/// </summary>
	private readonly ApplicationDbContext _dbContext;

	/// <summary>
	/// Контекст текущего пользователя.
	/// </summary>
	private readonly ICurrentUser _currentUser;

	/// <summary>
	/// Контекст текущей организации.
	/// </summary>
	private readonly ICurrentOrganization _currentOrganization;

	/// <summary>
	/// Сервис проверки прав.
	/// </summary>
	private readonly IPermissionChecker _permissionChecker;

	/// <summary>
	/// Валидатор команды создания.
	/// </summary>
	private readonly IValidator<CreateCategoryCommand> _commandValidator;

	/// <summary>
	/// Инициализирует обработчик создания категории.
	/// </summary>
	/// <param name="dbContext"> Контекст базы данных. </param>
	/// <param name="currentUser"> Контекст текущего пользователя. </param>
	/// <param name="currentOrganization"> Контекст текущей организации. </param>
	/// <param name="permissionChecker"> Сервис проверки прав. </param>
	/// <param name="commandValidator"> Валидатор команды создания. </param>
	public CreateCategoryHandler(
		ApplicationDbContext dbContext,
		ICurrentUser currentUser,
		ICurrentOrganization currentOrganization,
		IPermissionChecker permissionChecker,
		IValidator<CreateCategoryCommand> commandValidator)
	{
		ArgumentNullException.ThrowIfNull(dbContext);
		ArgumentNullException.ThrowIfNull(currentUser);
		ArgumentNullException.ThrowIfNull(currentOrganization);
		ArgumentNullException.ThrowIfNull(permissionChecker);
		ArgumentNullException.ThrowIfNull(commandValidator);

		_dbContext = dbContext;
		_currentUser = currentUser;
		_currentOrganization = currentOrganization;
		_permissionChecker = permissionChecker;
		_commandValidator = commandValidator;
	}

	/// <summary>
	/// Обрабатывает создание категории.
	/// </summary>
	/// <param name="command"> Команда создания категории. </param>
	/// <param name="cancellationToken"> Токен отмены операции. </param>
	/// <returns> Ответ получения конкретной категории. </returns>
	public async Task<Result<CategoryDetailsResponse>> Handle(
		CreateCategoryCommand command, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(command);

		await _currentOrganization.LoadContext(cancellationToken);

		if (!_currentOrganization.HasOrganization)
		{
			return Result.Failure<CategoryDetailsResponse>(
				CategoryErrors.OrganizationNotFound);
		}

		var permissionCheckResult = await _permissionChecker.HasPermission(
			Permissions.Categories.Create, cancellationToken);

		if (!permissionCheckResult)
		{
			return Result.Failure<CategoryDetailsResponse>(
				CategoryErrors.Forbidden);
		}

		var commandValidationResult = await _commandValidator
			.ValidateAsync(command, cancellationToken);

		if (!commandValidationResult.IsValid)
		{
			var validationErrorMessage = string.Join("; ", 
				commandValidationResult.Errors.Select(x => x.ErrorMessage));

			return Result.Failure<CategoryDetailsResponse>(
				CategoryErrors.BuildValidationError(validationErrorMessage));
		}

		// если в запросе указана родительская категория, то надо проверить 
		// существует ли она, является доступной для текущей организации
		// и совместима по типу категории и по типу активности.
		if (command.ParentCategoryId != null)
		{
			var parentCategory = await _dbContext.Categories
				.AsNoTracking()
				.FirstOrDefaultAsync(c =>
					c.Id == command.ParentCategoryId &&
					(c.OrganizationId == null ||
					c.OrganizationId == _currentOrganization.OrganizationId),
					cancellationToken);

			if (parentCategory == null)
			{
				return Result.Failure<CategoryDetailsResponse>(
					CategoryErrors.ParentCategoryNotFound);
			}

			if (parentCategory.CategoryType != command.CategoryType ||
				parentCategory.ActivityType != command.ActivityType)
			{
				return Result.Failure<CategoryDetailsResponse>(
					CategoryErrors.ParentCategoryTypeConflict);
			}
		}

		var newCategory = new Category(
			command.Name,
			command.CategoryType,
			command.ActivityType,
			command.Description,
			_currentOrganization.OrganizationId,
			command.ParentCategoryId);

		newCategory.MarkCreated(_currentUser.RequiredUserId);

		_dbContext.Categories.Add(newCategory);
		await _dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success(
			CategoryDetailsResponse.CreateFromEntity(newCategory));
	}
}
