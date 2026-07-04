using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FinaccApi.Modules.Categories.Domain;
using FinaccApi.Modules.Organizations.Domain;
using FinaccApi.DataAccess.Configurations.Shared;

namespace FinaccApi.DataAccess.Configurations;

/// <summary>
/// Конфигурация для модели Category.
/// </summary>
public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    { 
        builder.ToTable("categories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.CategoryType)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.ActivityType)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

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
