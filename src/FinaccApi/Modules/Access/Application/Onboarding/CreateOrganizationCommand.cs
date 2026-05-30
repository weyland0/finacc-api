namespace FinaccApi.Modules.Access.Application.Onboarding;

/// <summary>
/// Команда приложения для создания новой организации.
/// </summary>
public class CreateOrganizationCommand
{
    /// <summary>
    /// Краткое название организации.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Полное юридическое название организации.
    /// </summary>
    public string? LegalEntityName { get; set; }

    /// <summary>
    /// Регистрационный номер организации: ОГРН для юридических лиц или ОГРНИП для индивидуальных предпринимателей.
    /// </summary>
    public string? RegistrationNumber { get; set; }

    /// <summary>
    /// Налоговый идентификатор организации, например ИНН.
    /// </summary>
    public string? TaxId { get; set; }

    /// <summary>
    /// Полный юридический адрес организации.
    /// </summary>
    public string? FullAddress { get; set; }

    /// <summary>
    /// Контактная электронная почта организации.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Контактный телефон организации.
    /// </summary>
    public string? Phone { get; set; }
}
