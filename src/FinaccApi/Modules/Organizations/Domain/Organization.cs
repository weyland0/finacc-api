using FinaccApi.Shared.Kernel;

namespace FinaccApi.Modules.Organizations.Domain;

/// <summary>
/// Представляет организацию, в которой пользователи ведут финансовый учет.
/// </summary>
public class Organization : AuditableEntity
{
    /// <summary>
    /// Краткое название организации.
    /// </summary>
    public string Name { get; private set; } = null!;

    /// <summary>
    /// Полное юридическое название организации.
    /// </summary>
    public string? LegalEntityName { get; private set; }

    /// <summary>
    /// Регистрационный номер организации: ОГРН для юридических лиц или ОГРНИП для индивидуальных предпринимателей.
    /// </summary>
    public string? RegistrationNumber { get; private set; }

    /// <summary>
    /// Налоговый идентификатор организации, например ИНН.
    /// </summary>
    public string? TaxId { get; private set; }

    /// <summary>
    /// Полный юридический адрес организации.
    /// </summary>
    public string? FullAddress { get; private set; }

    /// <summary>
    /// Контактная электронная почта организации.
    /// </summary>
    public string? Email { get; private set; }

    /// <summary>
    /// Контактный телефон организации.
    /// </summary>
    public string? Phone { get; private set; }

    /// <summary>
    /// Создает организацию с обязательным кратким названием.
    /// </summary>
    /// <param name="name">Краткое название организации.</param>
    public Organization(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Создает организацию с полным набором регистрационных и контактных данных.
    /// </summary>
    /// <param name="name">Краткое название организации.</param>
    /// <param name="legalEntityName">Полное юридическое название организации.</param>
    /// <param name="registrationNumber">Регистрационный номер организации.</param>
    /// <param name="taxId">Налоговый идентификатор организации.</param>
    /// <param name="fullAddress">Полный юридический адрес организации.</param>
    /// <param name="email">Контактная электронная почта организации.</param>
    /// <param name="phone">Контактный телефон организации.</param>
    public Organization(
        string name,
        string? legalEntityName,
        string? registrationNumber,
        string? taxId,
        string? fullAddress,
        string? email,
        string? phone)
    {
        Name = name;
        LegalEntityName = legalEntityName;
        RegistrationNumber = registrationNumber;
        TaxId = taxId;
        FullAddress = fullAddress;
        Email = email;
        Phone = phone;
    }

    public void Update(
        string? name,
        string? legalEntityName,
        string? registrationNumber,
        string? taxId,
        string? fullAddress,
        string? email,
        string? phone)
    {
        Name = name ?? Name;
        LegalEntityName = legalEntityName ?? LegalEntityName;
        RegistrationNumber = registrationNumber ?? RegistrationNumber;
        TaxId = taxId ?? TaxId;
        FullAddress = fullAddress ?? FullAddress;
        Email = email ?? Email;
        Phone = phone ?? Phone;
    }
}
