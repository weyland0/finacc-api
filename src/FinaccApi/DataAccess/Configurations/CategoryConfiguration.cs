using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FinaccApi.Modules.Categories.Domain;
using FinaccApi.Modules.Organizations.Domain;
using FinaccApi.DataAccess.Configurations.Shared;

namespace FinaccApi.DataAccess.Configurations;

/// <summary>
/// Конфигурация модели <see cref="Category"/>.
/// </summary>
public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
	/// <summary>
	/// Имя таблицы.
	/// </summary>
	public const string TableName = "categories";

	/// <summary>
	/// Конфигурирует модель.
	/// </summary>
	/// <param name="builder"> Построитель модели. </param>
	public void Configure(EntityTypeBuilder<Category> builder)
	{
		builder.ToTable(TableName);

		builder.HasKey(x => x.Id);

		builder.Property(x => x.Id)
			.ValueGeneratedOnAdd()
			.IsRequired();

		builder.Property(x => x.Name)
			.HasMaxLength(CategoryConstraints.MaxNameLength)
			.IsRequired();

		builder.Property(x => x.CategoryType)
			.HasConversion(
				value => value.ToString().ToLowerInvariant(),
				value => Enum.Parse<CategoryType>(value, true))
			.HasMaxLength(CategoryConstraints.MaxCategoryTypeLength)
			.IsRequired();

		builder.Property(x => x.ActivityType)
			.HasConversion(
				value => value.ToString().ToLowerInvariant(),
				value => Enum.Parse<ActivityType>(value, true))
			.HasMaxLength(CategoryConstraints.MaxActivityTypeLength)
			.IsRequired();

		builder.Property(x => x.Description)
			.HasMaxLength(CategoryConstraints.MaxDescriptionLength);

		builder.Property(x => x.OrganizationId)
			.IsRequired(false);

		builder.Property(x => x.ParentId)
			.IsRequired(false);

		AuditableEntityConfigurationExtension.ConfigureAuditableEntity(builder);

		builder.HasIndex(x => x.OrganizationId);

		builder.HasIndex(x => new { x.OrganizationId, x.Name, x.CategoryType });

		builder.HasOne<Organization>()
			.WithMany()
			.HasForeignKey(x => x.OrganizationId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.HasOne<Category>()
			.WithMany()
			.HasForeignKey(x => x.ParentId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}
