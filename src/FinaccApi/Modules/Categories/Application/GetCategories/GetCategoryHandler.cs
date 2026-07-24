using Microsoft.EntityFrameworkCore;
using FinaccApi.DataAccess;
using FinaccApi.Shared.Kernel;
using FinaccApi.Shared.Application.MultiTenancy;
using FinaccApi.Shared.Application.Auth;
using FinaccApi.Modules.Categories.Domain;
using FinaccApi.Modules.Access.Domain;

namespace FinaccApi.Modules.Categories.Application.GetCategories;

public sealed class GetCategoriesHandler
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICurrentOrganization _currentOrganization;
    private readonly IPermissionChecker _permissionChecker;

    public GetCategoriesHandler(
        ApplicationDbContext dbContext,
        ICurrentOrganization currentOrganization,
        IPermissionChecker permissionChecker)
    {
        _dbContext = dbContext;
        _currentOrganization = currentOrganization;
        _permissionChecker = permissionChecker;
    }

    public async Task<Result<IReadOnlyList<CategoryResponse>>> Handle(
        GetCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        await _currentOrganization.LoadContext(cancellationToken);

        if (!_currentOrganization.HasOrganization)
        {
            return Result.Failure<IReadOnlyList<CategoryResponse>>(
                CategoryErrors.OrganizationNotFound);
        }

        var checkPermissionResult = await _permissionChecker.HasPermission(
            Permissions.Categories.View,
            cancellationToken);

        if (!checkPermissionResult)
        {
            return Result.Failure<IReadOnlyList<CategoryResponse>>(CategoryErrors.Forbidden);
        }

        var organizationId = _currentOrganization.OrganizationId!.Value;

        var query = BuildQuery(request, organizationId);

        var categories = await query
            .OrderBy(category => category.Name)
            .ToListAsync(cancellationToken);

        IReadOnlyList<CategoryResponse> response = request.AsTree
            ? CategoryResponseMapper.BuildTree(categories, organizationId)
            : CategoryResponseMapper.BuildFlatList(categories, organizationId);

        return Result.Success(response);
    }

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
            var escapedSearch = EscapeLikePattern(request.Search.Trim());
            var pattern = $"%{escapedSearch}%";

            query = query.Where(category =>
                EF.Functions.ILike(
                    category.Name,
                    pattern,
                    "\\"));
        }

        return query;
    }

    private static IQueryable<Category> ApplyScopeFilter(
        IQueryable<Category> query,
        CategoryScope scope,
        int organizationId)
    {
        return scope switch
        {
            CategoryScope.AllAvailable => query,

            CategoryScope.OnlyGlobal =>
                query.Where(category =>
                    category.OrganizationId == null),

            CategoryScope.OnlyOrganization =>
                query.Where(category =>
                    category.OrganizationId == organizationId),

            _ => throw new ArgumentOutOfRangeException(
                nameof(scope),
                scope,
                "Unknown category scope.")
        };
    }

    private static string EscapeLikePattern(string value)
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
