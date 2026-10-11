using Microsoft.EntityFrameworkCore;
using FinaccApi.DataAccess;
using FinaccApi.Shared.Kernel;
using FinaccApi.Shared.Application.MultiTenancy;
using FinaccApi.Shared.Application.Auth;
using FinaccApi.Modules.Access.Domain;

namespace FinaccApi.Modules.Categories.Application.GetCategoryById;

/// <summary>
/// Обработчик получения категории по идентификатору.
/// </summary>
public sealed class GetCategoryByIdHandler
{
	/// <summary>
	/// Контекст базы данных.
	/// </summary>
	private readonly ApplicationDbContext _dbContext;

	/// <summary>
	/// Контекст текущей организации.
	/// </summary>
	private readonly ICurrentOrganization _currentOrganization;

	/// <summary>
	/// Сервис проверки прав.
	/// </summary>
	private readonly IPermissionChecker _permissionChecker;

	/// <summary>
	/// Инициализирует обработчик.
	/// </summary>
	/// <param name="dbContext"> Контекст базы данных. </param>
	/// <param name="currentOrganization"> Контекст текущей организации. </param>
	/// <param name="permissionChecker"> Сервис проверки прав. </param>
	public GetCategoryByIdHandler(
		ApplicationDbContext dbContext,
		ICurrentOrganization currentOrganization,
		IPermissionChecker permissionChecker)
	{
		ArgumentNullException.ThrowIfNull(dbContext);
		ArgumentNullException.ThrowIfNull(currentOrganization);
		ArgumentNullException.ThrowIfNull(permissionChecker);

		_dbContext = dbContext;
		_currentOrganization = currentOrganization;
		_permissionChecker = permissionChecker;
	}

	/// <summary>
	/// Обрабатывает сценарий получения категории по идентификатору.
	/// </summary>
	/// <param name="query"> Запрос получения категории по идентификатору. </param>
	/// <param name="cancellationToken"> Токен отмены операции. </param>
	/// <returns> Ответ получения конкретной категории. </returns>
	public async Task<Result<CategoryDetailsResponse>> Handle(
		GetCategoryByIdQuery query,
		CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(query);

		await _currentOrganization.LoadContext(cancellationToken);

		if (!_currentOrganization.HasOrganization)
		{
			return Result.Failure<CategoryDetailsResponse>(
				CategoryErrors.OrganizationNotFound);
		}

		var checkPermissionResult = await _permissionChecker.HasPermission(
			Permissions.Categories.View,
			cancellationToken);

		if (!checkPermissionResult)
		{
			return Result.Failure<CategoryDetailsResponse>(CategoryErrors.Forbidden);
		}

		var organizationId = _currentOrganization.OrganizationId!.Value;

		var category = await _dbContext.Categories
			.AsNoTracking()
			.SingleOrDefaultAsync(
				c => c.Id == query.CategoryId &&
					(c.OrganizationId == organizationId ||
					c.OrganizationId == null),
			cancellationToken);

		if (category == null)
		{
			return Result.Failure<CategoryDetailsResponse>(CategoryErrors.CategoryNotFound);
		}

		return Result.Success(
			CategoryDetailsResponse.CreateFromEntity(category));
	}
}
