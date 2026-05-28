namespace FinaccApi.Modules.Access.Application.Onboarding;

/// <summary>
/// API-модель данных для создания новой организации.
/// </summary>
public class CreateOrganizationCommand
{
    public string Name { get; set; } = null!;

    /// <summary>
    /// Юридическое название организации.
    /// </summary>
    public string? LegalEntityName { get; set; }

    /// <summary>
    /// Регистрационный номер организации или ОГРН -
    /// уникальный цифровой код, присваиваемый при 
    /// создании. Основным является ОГРН (13 цифр) для юрлиц 
    /// или ОГРНИП (15 цифр) для ИП
    /// </summary>
    public string? RegistrationNumber { get; set; }

    /// <summary>
    /// Налоговый идентификатор организации или ИНН - 
    /// уникальный цифровой код (10 цифр), 
    /// присваиваемый ФНС.
    /// </summary>
    public string? TaxId { get; set; }

    /// <summary>
    /// Юридический адрес организации.
    /// </summary>
    public string? FullAddress { get; set; }

    /// <summary>
    /// Электронная почта организации.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Контактный телефон организации.
    /// </summary>
    public string? Phone { get; set; }
}
