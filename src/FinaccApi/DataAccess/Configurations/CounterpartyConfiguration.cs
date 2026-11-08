using FinaccApi.DataAccess.Configurations.Shared;
using FinaccApi.Modules.Counterparties.Domain;
using FinaccApi.Modules.Organizations.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinaccApi.DataAccess.Configurations;

/// <summary>
/// Конфигурация модели контрагента.
/// </summary>
public sealed class CounterpartyConfiguration : IEntityTypeConfiguration<Counterparty>
{
	/// <summary>
	/// Настраивает конфигурацию модели контрагента.
	/// </summary>
	/// <param name="builder"> Построитель модели. </param>
	public void Configure(EntityTypeBuilder<Counterparty> builder)
	{
		builder.ToTable(CounterpartyConstraints.TableName);

		builder.HasKey(x => x.Id);
		builder.Property(x => x.Id).ValueGeneratedNever().IsRequired();

		builder.Property(x => x.OrganizationId).IsRequired();

		builder.Property(x => x.Name)
			.HasMaxLength(CounterpartyConstraints.MaxNameLength)
			.IsRequired();

		builder.Property(x => x.Type)
			.HasConversion(
				value => value!.Value.ToString().ToLowerInvariant(),
				value => Enum.Parse<CounterpartyType>(value, true))
			.HasMaxLength(CounterpartyConstraints.MaxTypeLength)
			.IsRequired(false);

		builder.Property(x => x.Category)
			.HasMaxLength(CounterpartyConstraints.MaxCategoryLength);

		builder.Property(x => x.Phone)
			.HasMaxLength(CounterpartyConstraints.MaxPhoneLength);

		builder.Property(x => x.Email)
			.HasMaxLength(CounterpartyConstraints.MaxEmailLength);

		builder.Property(x => x.IsArchived).HasDefaultValue(false).IsRequired();

		builder.ConfigureAuditableEntity();

		builder.HasIndex(x => x.OrganizationId);

		builder.HasOne<Organization>()
			.WithMany()
			.HasForeignKey(x => x.OrganizationId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}
