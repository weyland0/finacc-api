using FinaccApi.Shared.Kernel;

namespace FinaccApi.Modules.Organization.Domain;

/// <summary>
/// Модель организации.
/// </summary>
public class Organization : AuditableEntity
{
    /// <summary>
    /// Название организации.
    /// </summary>
    public string Name { get; private set; } = null!;

    /// <summary>
    /// Юридическое название организации.
    /// </summary>
    public string? LegalEntityName { get; private set; }

    /// <summary>
    /// Регистрационный номер организации или ОГРН -
    /// уникальный цифровой код, присваиваемый при 
    /// создании. Основным является ОГРН (13 цифр) для юрлиц 
    /// или ОГРНИП (15 цифр) для ИП
    /// </summary>
    public string? RegistrationNumber { get; private set; }

    /// <summary>
    /// Налоговый идентификатор организации или ИНН - 
    /// уникальный цифровой код (10 цифр), 
    /// присваиваемый ФНС.
    /// </summary>
    public string? TaxId { get; private set; }

    /// <summary>
    /// Юридический адрес организации.
    /// </summary>
    public string? FullAddress { get; private set; }

    /// <summary>
    /// Электронная почта организации.
    /// </summary>
    public string? Email { get; private set; }

    /// <summary>
    /// Контактный телефон организации.
    /// </summary>
    public string? Phone { get; private set; }
}
