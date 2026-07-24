using FinaccApi.Modules.Categories.Domain;

namespace FinaccApi.Modules.Categories.Application.GetCategories;

public class CategoryResponseMapper
{
    public static IReadOnlyList<CategoryResponse> BuildFlatList(
        IEnumerable<Category> categories,
        int currentOrganizationId)
    {
        return categories
            .Select(category => MapToResponse(
                category,
                currentOrganizationId,
                []))
            .ToList();
    }

    public static IReadOnlyList<CategoryResponse> BuildTree(IReadOnlyCollection<Category> categories, int currentOrganizationId)
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
            .Select(node => MapToNode(node, currentOrganizationId))
            .ToList();
    }

    private static CategoryResponse MapToNode(CategoryNode node, int currentOrganizationId)
    {
        var children = node.Children
            .OrderBy(child => child.Category.Name)
            .Select(child => MapToNode(child, currentOrganizationId))
            .ToList();

        return MapToResponse(
            node.Category,
            currentOrganizationId,
            children);
    }

    private static CategoryResponse MapToResponse(
        Category category,
        int currentOrganizationId,
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
            category.OrganizationId == currentOrganizationId,
            children);
    }

    private sealed class CategoryNode
    {
        public Category Category { get; }
        public List<CategoryNode> Children { get; } = [];

        public CategoryNode(Category category)
        {
            Category = category;
        }
    }
}
