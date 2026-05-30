using FinaccApi.Shared.Kernel;

namespace FinaccApi.Modules.Access.Domain;

/// <summary>
/// Описывает отдельное разрешение на выполнение действия в системе.
/// </summary>
public class Permission : Entity
{
    /// <summary>
    /// Уникальный код разрешения, например "Accounts.View" или "Invoices.Pay".
    /// </summary>
    public string Code { get; private set; } = null!;

    /// <summary>
    /// Текстовое описание действия, которое разрешает это право.
    /// </summary>
    public string? Description { get; private set; }
}
