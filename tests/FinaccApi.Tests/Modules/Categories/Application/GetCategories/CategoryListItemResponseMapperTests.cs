using FinaccApi.Modules.Categories.Application.GetCategories;
using FinaccApi.Modules.Categories.Domain;

namespace FinaccApi.Tests.Modules.Categories.Application.GetCategories;

[TestClass]
public sealed class CategoryListItemResponseMapperTests
{
	[TestMethod]
	public void BuildTree_WithGuidParentLinks_PreservesHierarchyAndNullableOwnership()
	{
		var organizationId = Guid.NewGuid();
		var root = new Category(
			"Global root", CategoryType.Expense, ActivityType.Operating,
			null, null, null);
		var child = new Category(
			"Organization child", CategoryType.Expense, ActivityType.Operating,
			null, organizationId, root.Id);
		var leaf = new Category(
			"Leaf", CategoryType.Expense, ActivityType.Operating,
			null, organizationId, child.Id);

		var tree = CategoryListItemResponseMapper.BuildTree([leaf, root, child]);

		var rootResponse = tree.Single();
		Assert.AreEqual(root.Id, rootResponse.Id);
		Assert.IsNull(rootResponse.OrganizationId);
		Assert.IsNull(rootResponse.ParentCategoryId);
		var childResponse = rootResponse.Children.Single();
		Assert.AreEqual(child.Id, childResponse.Id);
		Assert.AreEqual(root.Id, childResponse.ParentCategoryId);
		Assert.AreEqual(organizationId, childResponse.OrganizationId);
		var leafResponse = childResponse.Children.Single();
		Assert.AreEqual(leaf.Id, leafResponse.Id);
		Assert.AreEqual(child.Id, leafResponse.ParentCategoryId);
		Assert.AreEqual(organizationId, leafResponse.OrganizationId);
		Assert.AreEqual(0, leafResponse.Children.Count);
	}
}
