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

        var organizationId = _currentOrganization.OrganizationId!.Value;

        var checkPermissionResult = await _permissionChecker.HasPermission(
            Permissions.Categories.View,
            cancellationToken);

        if (!checkPermissionResult)
        {
            return Result.Failure<IReadOnlyList<CategoryResponse>>(CategoryErrors.Forbidden);
        }

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

        var categories = await query
            .OrderBy(category => category.Name)
            .ToListAsync(cancellationToken);

        IReadOnlyList<CategoryResponse> response = request.AsTree
            ? BuildTree(categories, organizationId)
            : BuildFlatList(categories, organizationId);

        return Result.Success(response);
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

    private static IReadOnlyList<CategoryResponse> BuildFlatList(
        IEnumerable<Category> categories,
        int organizationId)
    {
        return categories
            .Select(category => MapToResponse(
                category,
                organizationId,
                []))
            .ToList();
    }

    private static IReadOnlyList<CategoryResponse> BuildTree(
        IReadOnlyCollection<Category> categories,
        int organizationId)
    {
        var nodes = categories.ToDictionary(
            category => category.Id,
            category => new CategoryNode(category));

        var roots = new List<CategoryNode>();

        foreach (var node in nodes.Values)
        {
            var parentId = node.Category.ParentId;

            if (parentId.HasValue &&
                nodes.TryGetValue(parentId.Value, out var parent))
            {
                parent.Children.Add(node);
            }
            else
            {
                roots.Add(node);
            }
        }

        return roots
            .OrderBy(node => node.Category.Name)
            .Select(node => MapNode(node, organizationId))
            .ToList();
    }

    private static CategoryResponse MapNode(
        CategoryNode node,
        int organizationId)
    {
        var children = node.Children
            .OrderBy(child => child.Category.Name)
            .Select(child => MapNode(child, organizationId))
            .ToList();

        return MapToResponse(
            node.Category,
            organizationId,
            children);
    }

    private static CategoryResponse MapToResponse(
        Category category,
        int organizationId,
        IReadOnlyList<CategoryResponse> children)
    {
        return new CategoryResponse(
            category.Id,
            category.Name,
            category.CategoryType,
            category.ActivityType,
            category.Description,
            category.OrganizationId,
            category.ParentId,
            category.OrganizationId == null,
            category.OrganizationId == organizationId,
            children);
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

    private sealed class CategoryNode
    {
        public CategoryNode(Category category)
        {
            Category = category;
        }

        public Category Category { get; }

        public List<CategoryNode> Children { get; } = [];
    }
}
