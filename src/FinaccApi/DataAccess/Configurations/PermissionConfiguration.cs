using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FinaccApi.Modules.Access.Domain;

namespace FinaccApi.DataAccess.Configurations;

/// <summary>
/// Конфигурация для модели Permission.
/// </summary>
public sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.HasIndex(x => x.Code)
            .IsUnique();
    }
}
