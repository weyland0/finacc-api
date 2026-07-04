using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FinaccApi.Modules.Access.Domain;

namespace FinaccApi.DataAccess.Configurations;

/// <summary>
/// Конфигурация для модели Role.
/// </summary>
public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.Code)
            .IsUnique();
    }
}
