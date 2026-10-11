using Microsoft.EntityFrameworkCore;
using FinaccApi.DataAccess;
using FinaccApi.Shared.Kernel;
using FinaccApi.Shared.Application.MultiTenancy;
using FinaccApi.Shared.Application.Auth;
using FinaccApi.Modules.Categories.Domain;
using FinaccApi.Modules.Access.Domain;

namespace FinaccApi.Modules.Categories.Application.GetCategories;

/// <summary>
/// Обработчик получения категорий.
/// </summary>
public sealed class GetCategoriesHandler
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
	public GetCategoriesHandler(
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
	/// Обрабатывает получение категорий.
	/// </summary>
	/// <param name="request"> Запрос на получения категорий. </param>
	/// <param name="cancellationToken"> Токен отмены операции. </param>
	/// <returns> Коллекцию категорий. </returns>
	public async Task<Result<IReadOnlyList<CategoryListItemResponse>>> Handle(
		GetCategoriesQuery request,
		CancellationToken cancellationToken)
	{
		await _currentOrganization.LoadContext(cancellationToken);

		if (!_currentOrganization.HasOrganization)
		{
			return Result.Failure<IReadOnlyList<CategoryListItemResponse>>(
				CategoryErrors.OrganizationNotFound);
		}

		var checkPermissionResult = await _permissionChecker.HasPermission(
			Permissions.Categories.View,
			cancellationToken);

		if (!checkPermissionResult)
		{
			return Result.Failure<IReadOnlyList<CategoryListItemResponse>>(CategoryErrors.Forbidden);
		}

		var organizationId = _currentOrganization.OrganizationId!.Value;

		var query = BuildQuery(request, organizationId);

		var categories = await query
			.OrderBy(category => category.Name)
			.ToListAsync(cancellationToken);

		IReadOnlyList<CategoryListItemResponse> response = request.AsTree
			? CategoryListItemResponseMapper.BuildTree(categories)
			: CategoryListItemResponseMapper.BuildFlatList(categories);

		return Result.Success(response);
	}

	/// <summary>
	/// Создает запрос на получение категорий по указанным в
	/// <paramref name="request"/> параметрам.
	/// </summary>
	/// <param name="request"> Запрос на получение категорий. </param>
	/// <param name="organizationId"> Идентификатор организации. </param>
	/// <returns> Запрос в БД на получение категорий. </returns>
	private IQueryable<Category> BuildQuery(
		GetCategoriesQuery request,
		int organizationId)
	{
		IQueryable<Category> query = _dbContext.Categories
			.AsNoTracking()
			.Where(category =>
				category.OrganizationId == null ||
				category.OrganizationId == organizationId);

		query = ApplyScopeFilter(
			query,
			request.Scope,
			organizationId);

		if (request.CategoryType.HasValue)
		{
			var categoryType = request.CategoryType.Value;

			query = query.Where(category =>
				category.CategoryType == categoryType);
		}

		if (request.ActivityType.HasValue)
		{
			var activityType = request.ActivityType.Value;

			query = query.Where(category =>
				category.ActivityType == activityType);
		}

		if (!string.IsNullOrWhiteSpace(request.Search))
		{
			var escapedSearch = WrapEscapeLikePattern(request.Search.Trim());
			var pattern = $"%{escapedSearch}%";

			query = query.Where(category =>
				EF.Functions.ILike(
					category.Name,
					pattern,
					"\\"));
		}

		return query;
	}

	/// <summary>
	/// Применяет к <paramref name="query"/> область поиска категорий.
	/// </summary>
	/// <param name="query"> Запрос в БД на получение категорий. </param>
	/// <param name="scope"> Область поиска категорий. </param>
	/// <param name="organizationId"> Идентификатор организации. </param>
	/// <returns> Запрос в БД на получение категорий. </returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Выбрасывает, если передана некорректная область поиска.
	/// </exception>
	private static IQueryable<Category> ApplyScopeFilter(
		IQueryable<Category> query,
		CategorySearchScope scope,
		int organizationId)
	{
		return scope switch
		{
			CategorySearchScope.AllAvailable => query,

			CategorySearchScope.OnlyGlobal =>
				query.Where(category =>
					category.OrganizationId == null),

			CategorySearchScope.OnlyOrganization =>
				query.Where(category =>
					category.OrganizationId == organizationId),

			_ => throw new ArgumentOutOfRangeException(
				nameof(scope),
				scope,
				"Unknown category scope.")
		};
	}

	/// <summary>
	/// Оборачивает спец. символы в Escape Like Pattern.
	/// </summary>
	/// <param name="value"> Строка поиска. </param>
	/// <returns> Обернутая строка поиска. </returns>
	private static string WrapEscapeLikePattern(string value)
	{
		return value
			.Replace("\\", "\\\\")
			.Replace("%", "\\%")
			.Replace("_", "\\_")
			.Replace("[", "\\[")
			.Replace("]", "\\]")
			.Replace("^", "\\^");
	}
}
