using FinaccApi.DataAccess;
using FinaccApi.Modules.Access.Domain;
using FinaccApi.Modules.Counterparties.Domain;
using FinaccApi.Modules.Organizations.Domain;
using Microsoft.EntityFrameworkCore;

namespace FinaccApi.Tests.Modules.Counterparties.DataAccess;

/// <summary>
/// Проверки метаданных PostgreSQL-модели без подключения к базе данных.
/// </summary>
[TestClass]
public sealed class CounterpartyConfigurationTests
{
	/// <summary>
	/// Проверяет структуру, nullable-поля, длины, ограничение удаления организации и индексы модели.
	/// </summary>
	[TestMethod]
	public void Model_WhenConfigured_MapsTenantFieldsAndRestrictsOrganizationDeletion()
	{
		using var context = CreatePostgresModelContext();
		var entity = context.Model.FindEntityType(typeof(Counterparty))!;

		Assert.AreEqual("counterparties", entity.GetTableName());
		Assert.AreEqual(nameof(Counterparty.Id), entity.FindPrimaryKey()!.Properties.Single().Name);
		Assert.IsFalse(entity.FindProperty(nameof(Counterparty.OrganizationId))!.IsNullable);
		Assert.IsFalse(entity.FindProperty(nameof(Counterparty.Name))!.IsNullable);
		Assert.AreEqual(CounterpartyConstraints.MaxNameLength, entity.FindProperty(nameof(Counterparty.Name))!.GetMaxLength());
		Assert.AreEqual(CounterpartyConstraints.MaxCategoryLength, entity.FindProperty(nameof(Counterparty.Category))!.GetMaxLength());
		Assert.AreEqual(CounterpartyConstraints.MaxPhoneLength, entity.FindProperty(nameof(Counterparty.Phone))!.GetMaxLength());
		Assert.AreEqual(CounterpartyConstraints.MaxEmailLength, entity.FindProperty(nameof(Counterparty.Email))!.GetMaxLength());
		foreach (var propertyName in new[] { nameof(Counterparty.Type), nameof(Counterparty.Category), nameof(Counterparty.Phone), nameof(Counterparty.Email) })
			Assert.IsTrue(entity.FindProperty(propertyName)!.IsNullable, propertyName);
		Assert.IsFalse(entity.FindProperty(nameof(Counterparty.IsArchived))!.IsNullable);
		Assert.AreEqual(false, entity.FindProperty(nameof(Counterparty.IsArchived))!.GetDefaultValue());

		var organizationForeignKey = entity.GetForeignKeys().Single(x => x.PrincipalEntityType.ClrType == typeof(Organization));
		Assert.AreEqual(nameof(Counterparty.OrganizationId), organizationForeignKey.Properties.Single().Name);
		Assert.AreEqual(DeleteBehavior.Restrict, organizationForeignKey.DeleteBehavior);
		Assert.IsTrue(entity.GetIndexes().Any(x => x.Properties.Count == 1 && x.Properties.Single().Name == nameof(Counterparty.OrganizationId)));
		Assert.IsFalse(entity.GetIndexes().Any(x => x.IsUnique && x.Properties.Any(p => p.Name == nameof(Counterparty.Name))));
		Assert.IsNotNull(entity.FindProperty(nameof(Counterparty.CreatedAt)));
		Assert.IsNotNull(entity.FindProperty(nameof(Counterparty.UpdatedAt)));
		Assert.AreEqual(2, entity.GetForeignKeys().Count(x => x.PrincipalEntityType.ClrType == typeof(User) && x.DeleteBehavior == DeleteBehavior.NoAction));
	}

	/// <summary>
	/// Проверяет строковое преобразование nullable enum в PostgreSQL с сохранением null.
	/// </summary>
	[TestMethod]
	public void TypeConversion_WhenNullableEnum_RoundTripsValuesAndNull()
	{
		using var context = CreatePostgresModelContext();
		var property = context.Model.FindEntityType(typeof(Counterparty))!.FindProperty(nameof(Counterparty.Type))!;
		var converter = property.GetTypeMapping().Converter!;

		Assert.AreEqual(typeof(string), converter.ProviderClrType);
		Assert.AreEqual("character varying(50)", property.GetColumnType());
		Assert.IsNull(converter.ConvertToProvider(null));
		Assert.IsNull(converter.ConvertFromProvider(null));
		Assert.AreEqual("individual", converter.ConvertToProvider(CounterpartyType.Individual));
		Assert.AreEqual("legalentity", converter.ConvertToProvider(CounterpartyType.LegalEntity));
		Assert.AreEqual(CounterpartyType.Individual, converter.ConvertFromProvider("individual"));
		Assert.AreEqual(CounterpartyType.LegalEntity, converter.ConvertFromProvider("legalentity"));
	}

	private static ApplicationDbContext CreatePostgresModelContext()
		=> new(new DbContextOptionsBuilder<ApplicationDbContext>()
			.UseNpgsql("Host=localhost;Database=counterparty_model_tests;Username=model;Password=model")
			.UseSnakeCaseNamingConvention()
			.Options);
}
