using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FinaccApi.Shared.Kernel;
using FinaccApi.Modules.Access.Domain;

namespace FinaccApi.DataAccess.Configurations.Shared;

/// <summary>
/// Расширение для конфигурации модели AuditableEntity.
/// </summary>
public static class AuditableEntityConfigurationExtension
{
    /// <summary>
    /// Конфигурирует модель AuditableEntity.
    /// </summary>
    /// <typeparam name="TEntity">Тип модели.</typeparam>
    /// <param name="builder">Сборщик модели.</param>
    public static void ConfigureAuditableEntity<TEntity>(
        this EntityTypeBuilder<TEntity> builder)
        where TEntity : AuditableEntity
    {
        // Настройка свойств модели
        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .IsRequired();

        builder.Property(x => x.UpdatedBy)
            .IsRequired();

        // Настройка связей модели
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UpdatedBy)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
