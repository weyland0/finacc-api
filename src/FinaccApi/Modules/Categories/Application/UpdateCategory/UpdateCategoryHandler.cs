using Microsoft.EntityFrameworkCore;
using FinaccApi.DataAccess;
using FinaccApi.Shared.Kernel;
using FinaccApi.Shared.Application.Auth;
using FinaccApi.Shared.Application.MultiTenancy;
using FinaccApi.Modules.Access.Domain;
using FluentValidation;

namespace FinaccApi.Modules.Categories.Application.UpdateCategory;

/// <summary>
/// Обработчик обновления категории.
/// </summary>
public sealed class UpdateCategoryHandler
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
	/// Валидатор команды обновления.
	/// </summary>
	private readonly IValidator<UpdateCategoryCommand> _commandValidator;

	/// <summary>
	/// Инициализирует обработчик обновления категории.
	/// </summary>
	/// <param name="dbContext"> Контекст базы данных. </param>
	/// <param name="currentUser"> Контекст текущего пользователя. </param>
	/// <param name="currentOrganization"> Контекст текущей организации. </param>
	/// <param name="permissionChecker"> Сервис проверки прав. </param>
	/// <param name="commandValidator"> Валидатор команды обновления. </param>
	public UpdateCategoryHandler(
		ApplicationDbContext dbContext,
		ICurrentUser currentUser,
		ICurrentOrganization currentOrganization,
		IPermissionChecker permissionChecker,
		IValidator<UpdateCategoryCommand> commandValidator)
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
	/// Обновляет категорию текущей организации.
	/// </summary>
	/// <param name="command"> Команда обновления категории. </param>
	/// <param name="cancellationToken"> Токен отмены операции. </param>
	/// <returns> Результат обновления категории. </returns>
	public async Task<Result<CategoryDetailsResponse>> Handle(
		UpdateCategoryCommand command,
		CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(command);

		await _currentOrganization.LoadContext(cancellationToken);

		if (!_currentOrganization.HasOrganization)
		{
			return Result.Failure<CategoryDetailsResponse>(
				CategoryErrors.OrganizationNotFound);
		}

		var permissionCheckResult = await _permissionChecker.HasPermission(
			Permissions.Categories.Update, cancellationToken);

		if (!permissionCheckResult)
		{
			return Result.Failure<CategoryDetailsResponse>(CategoryErrors.Forbidden);
		}

		var commandValidationResult = await _commandValidator
			.ValidateAsync(command, cancellationToken);

		if (!commandValidationResult.IsValid)
		{
			return Result.Failure<CategoryDetailsResponse>(
				CategoryErrors.BuildValidationError(commandValidationResult.ToString("; ")));
		}

		var organizationId = _currentOrganization.OrganizationId!.Value;

		var category = await _dbContext.Categories.SingleOrDefaultAsync(c =>
			c.Id == command.CategoryId &&
			(c.OrganizationId == null || c.OrganizationId == organizationId),
			cancellationToken);

		if (category == null)
		{
			return Result.Failure<CategoryDetailsResponse>(CategoryErrors.CategoryNotFound);
		}

		if (category.IsGlobal())
		{
			return Result.Failure<CategoryDetailsResponse>(CategoryErrors.GlobalCategoryReadOnly);
		}

		category.UpdateDetails(command.Name, command.Description);
		category.MarkUpdated(_currentUser.RequiredUserId);

		await _dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success(CategoryDetailsResponse.CreateFromEntity(category));
	}
}
