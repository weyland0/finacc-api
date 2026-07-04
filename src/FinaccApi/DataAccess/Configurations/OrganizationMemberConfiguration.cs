using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FinaccApi.DataAccess.Configurations.Shared;
using FinaccApi.Modules.Access.Domain;
using FinaccApi.Modules.Organizations.Domain;

namespace FinaccApi.DataAccess.Configurations;

/// <summary>
/// Конфигурация для модели OrganizationMember.
/// </summary>
public sealed class OrganizationMemberConfiguration : IEntityTypeConfiguration<OrganizationMember>
{
    public void Configure(EntityTypeBuilder<OrganizationMember> builder)
    {
        builder.ToTable("organization_members");

        builder.HasKey(x => x.Id);

        AuditableEntityConfigurationExtension.ConfigureAuditableEntity(builder);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Role>()
            .WithMany()
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
