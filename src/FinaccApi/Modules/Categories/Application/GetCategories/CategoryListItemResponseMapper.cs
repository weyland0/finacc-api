using FinaccApi.Modules.Categories.Domain;

namespace FinaccApi.Modules.Categories.Application.GetCategories;

/// <summary>
/// Преобразователь ответа получения категорий.
/// </summary>
public class CategoryListItemResponseMapper
{
	/// <summary>
	/// Преобразует коллекцию категорий в коллекцию ответов получения категорий.
	/// </summary>
	/// <param name="categories"> Коллекция категорий. </param>
	/// <returns> Коллекцию ответов получения категорий. </returns>
	public static IReadOnlyList<CategoryListItemResponse> BuildFlatList(
		IEnumerable<Category> categories)
	{
		return categories
			.Select(category => MapToResponse(
				category,
				[]))
			.ToList();
	}

	/// <summary>
	/// Преобразует коллекцию категорий в коллекцию ответов получения категорий,
	/// организованную как дерево.
	/// </summary>
	/// <param name="categories"> Коллекция категорий. </param>
	/// <returns> Коллекцию ответов получения категорий в виде дерева. </returns>
	public static IReadOnlyList<CategoryListItemResponse> BuildTree(
		IReadOnlyCollection<Category> categories)
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
			.Select(node => MapToNode(node))
			.ToList();
	}

	/// <summary>
	/// Преобразует узел дерева категорий в элемент ответа получения категорий.
	/// </summary>
	/// <param name="node"> Узел дерева категорий. </param>
	/// <returns> Элемент ответа получения категорий. </returns>
	private static CategoryListItemResponse MapToNode(
		CategoryNode node)
	{
		var children = node.Children
			.OrderBy(child => child.Category.Name)
			.Select(child => MapToNode(child))
			.ToList();

		return MapToResponse(
			node.Category,
			children);
	}

	/// <summary>
	/// Преобразует категорию в элемент ответа получения категорий.
	/// </summary>
	/// <param name="category"> Категория. </param>
	/// <param name="children"> Дочерние категории. </param>
	/// <returns> Элемент ответа получения категорий. </returns>
	private static CategoryListItemResponse MapToResponse(
		Category category,
		IReadOnlyList<CategoryListItemResponse> children)
	{
		return new CategoryListItemResponse(
			category.Id,
			category.Name,
			category.CategoryType,
			category.ActivityType,
			category.Description,
			category.OrganizationId,
			category.ParentId,
			children);
	}

	/// <summary>
	/// Узел дерева категорий.
	/// </summary>
	private sealed class CategoryNode
	{
		/// <summary>
		/// Категория.
		/// </summary>
		public Category Category { get; }

		/// <summary>
		/// Дочерние категории.
		/// </summary>
		public List<CategoryNode> Children { get; } = [];

		/// <summary>
		/// Инициализирует узел дерева категорий.
		/// </summary>
		/// <param name="category"> Категория. </param>
		public CategoryNode(Category category)
		{
			Category = category;
		}
	}
}
