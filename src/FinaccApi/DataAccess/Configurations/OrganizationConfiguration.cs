using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FinaccApi.DataAccess.Configurations.Shared;
using FinaccApi.Modules.Organizations.Domain;

namespace FinaccApi.DataAccess.Configurations;

/// <summary>
/// Конфигурация для модели Organization.
/// </summary>
public sealed class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("organizations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.LegalEntityName)
            .HasMaxLength(200);

        builder.Property(x => x.RegistrationNumber)
            .HasMaxLength(50);

        builder.Property(x => x.TaxId)
            .HasMaxLength(50);

        builder.Property(x => x.FullAddress)
            .HasMaxLength(200);

        builder.Property(x => x.Email)
            .HasMaxLength(50);

        builder.Property(x => x.Phone)
            .HasMaxLength(50);

        AuditableEntityConfigurationExtension.ConfigureAuditableEntity(builder);

        builder.HasIndex(x => x.Name);

        builder.HasIndex(x => x.TaxId);
    }
}
