using FinaccApi.DataAccess;
using FinaccApi.Modules.Access.Domain;
using FinaccApi.Modules.Categories.Domain;
using FinaccApi.Modules.Counterparties.Domain;
using FinaccApi.Modules.Organizations.Domain;
using FinaccApi.Shared.Kernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FinaccApi.Tests.DataAccess;

/// <summary>
/// Проверки единого формата ключей и доступности идентификаторов до сохранения.
/// </summary>
[TestClass]
public sealed class UuidIdentityTests
{
	[TestMethod]
	public void NewEntities_BeforeTracking_HaveDistinctVersion7Ids()
	{
		var user = new User("owner@example.com", "123", "hash", "First", "Last");
		var organization = new Organization("Organization");
		var role = new Role("owner", "Owner");
		Entity[] entities =
		[
			user,
			organization,
			role,
			new Permission(),
			new OrganizationMember(user.Id, organization.Id, role.Id),
			new Category("Category", CategoryType.Income, ActivityType.Operating),
			new Counterparty(organization.Id, "Counterparty", null, null, null, null)
		];

		foreach (var entity in entities)
		{
			Assert.AreNotEqual(Guid.Empty, entity.Id, entity.GetType().Name);
			Assert.AreEqual('7', entity.Id.ToString("D")[14], entity.GetType().Name);
		}
		Assert.AreEqual(entities.Length, entities.Select(x => x.Id).Distinct().Count());
	}

	[TestMethod]
	public async Task SaveChanges_PreservesIdsUsedByNewRelatedEntities()
	{
		await using var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options);
		var user = new User("owner@example.com", "123", "hash", "First", "Last");
		var organization = new Organization("Organization");
		var role = new Role("owner", "Owner");
		organization.MarkCreated(user.Id);
		var member = new OrganizationMember(user.Id, organization.Id, role.Id);
		member.MarkCreated(user.Id);
		var userId = user.Id;
		var organizationId = organization.Id;
		var roleId = role.Id;
		var memberId = member.Id;
		context.AddRange(user, organization, role, member);

		await context.SaveChangesAsync();
		context.ChangeTracker.Clear();

		var savedMember = await context.OrganizationMembers.SingleAsync();
		Assert.AreEqual(memberId, savedMember.Id);
		Assert.AreEqual(userId, savedMember.UserId);
		Assert.AreEqual(organizationId, savedMember.OrganizationId);
		Assert.AreEqual(roleId, savedMember.RoleId);
		Assert.AreEqual(userId, savedMember.CreatedBy);
		Assert.AreEqual(userId, (await context.Users.SingleAsync()).Id);
		Assert.AreEqual(organizationId, (await context.Organizations.SingleAsync()).Id);
		Assert.AreEqual(roleId, (await context.Roles.SingleAsync()).Id);
	}

	[TestMethod]
	public void PostgresModel_AllPrimaryAndForeignKeys_UseApplicationAssignedUuid()
	{
		using var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
			.UseNpgsql("Host=localhost;Database=uuid_model_tests;Username=model;Password=model")
			.UseSnakeCaseNamingConvention()
			.Options);
		var entityTypes = context.Model.GetEntityTypes().ToArray();
		Assert.AreEqual(8, entityTypes.Length);

		foreach (var entityType in entityTypes)
		{
			var primaryKey = entityType.FindPrimaryKey()!;
			var keyProperties = primaryKey.Properties
				.Concat(entityType.GetForeignKeys().SelectMany(x => x.Properties))
				.Distinct();
			foreach (var property in keyProperties)
			{
				var message = $"{entityType.ClrType.Name}.{property.Name}";
				Assert.AreEqual(typeof(Guid), Nullable.GetUnderlyingType(property.ClrType) ?? property.ClrType, message);
				Assert.AreEqual("uuid", property.GetColumnType(), message);
				Assert.AreEqual(ValueGenerated.Never, property.ValueGenerated, message);
			}
		}
		var category = context.Model.FindEntityType(typeof(Category))!;
		Assert.IsTrue(category.FindProperty(nameof(Category.OrganizationId))!.IsNullable);
		Assert.IsTrue(category.FindProperty(nameof(Category.ParentId))!.IsNullable);
		var rolePermission = context.Model.FindEntityType(typeof(RolePermission))!;
		CollectionAssert.AreEqual(
			new[] { nameof(RolePermission.RoleId), nameof(RolePermission.PermissionId) },
			rolePermission.FindPrimaryKey()!.Properties.Select(x => x.Name).ToArray());
	}
}
