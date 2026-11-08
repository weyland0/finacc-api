using FinaccApi.Shared.Kernel;

namespace FinaccApi.Modules.Counterparties.Domain;

/// <summary>
/// Контрагент.
/// </summary>
public sealed class Counterparty : AuditableEntity, IOrganizationOwnedEntity
{
	/// <summary>
	/// Идентификатор организации.
	/// </summary>
	public int OrganizationId { get; private set; }

	/// <summary>
	/// Название или ФИО контрагента.
	/// </summary>
	public string Name { get; private set; } = null!;

	/// <summary>
	/// Тип физического или юридического лица.
	/// </summary>
	public CounterpartyType? Type { get; private set; }

	/// <summary>
	/// Бизнес-классификация контрагента.
	/// </summary>
	public string? Category { get; private set; }

	/// <summary>
	/// Номер телефона.
	/// </summary>
	public string? Phone { get; private set; }

	/// <summary>
	/// Адрес электронной почты.
	/// </summary>
	public string? Email { get; private set; }

	/// <summary>
	/// Признак архивирования контрагента.
	/// </summary>
	public bool IsArchived { get; private set; }

	private Counterparty()
	{
	}

	/// <summary>
	/// Создает контрагента организации.
	/// </summary>
	/// <param name="organizationId">Идентификатор организации-владельца.</param>
	/// <param name="name">Название или ФИО.</param>
	/// <param name="type">Тип контрагента.</param>
	/// <param name="category">Бизнес-классификация.</param>
	/// <param name="phone">Номер телефона.</param>
	/// <param name="email">Адрес электронной почты.</param>
	public Counterparty(
		int organizationId,
		string name,
		CounterpartyType? type,
		string? category,
		string? phone,
		string? email)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(organizationId);
		ArgumentException.ThrowIfNullOrWhiteSpace(name);

		OrganizationId = organizationId;
		UpdateDetails(name, type, category, phone, email);
	}

	/// <summary>
	/// Полностью заменяет данные активного контрагента, включая необязательные поля.
	/// </summary>
	/// <param name="name">Название или ФИО.</param>
	/// <param name="type">Тип контрагента или null для очистки.</param>
	/// <param name="category">Бизнес-классификация или null для очистки.</param>
	/// <param name="phone">Номер телефона или null для очистки.</param>
	/// <param name="email">Адрес электронной почты или null для очистки.</param>
	public void UpdateDetails(
		string name,
		CounterpartyType? type,
		string? category,
		string? phone,
		string? email)
	{
		if (IsArchived)
		{
			throw new InvalidOperationException(
				"Архивированного контрагента нельзя редактировать.");
		}

		ArgumentException.ThrowIfNullOrWhiteSpace(name);

		Name = name;
		Type = type;
		Category = category;
		Phone = phone;
		Email = email;
	}

	/// <summary>
	/// Архивирует контрагента.
	/// </summary>
	public void Archive()
	{
		if (IsArchived)
		{
			return;
		}

		IsArchived = true;
	}
}
