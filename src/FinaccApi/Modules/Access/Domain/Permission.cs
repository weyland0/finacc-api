using FinaccApi.Shared.Kernel;

namespace FinaccApi.Modules.Access.Domain;

/// <summary>
/// Модель прав доступа.
/// Хранит конкретные разрешения на конкретное действие в системе.
/// </summary>
public class Permission : Entity
{
    /// <summary>
    /// Код разрешения на выполнение действия.
    /// Например: "Accounts.View", "Invoices.Pay" и т.д.
    /// </summary>
    public string Code { get; private set; } = null!;

    /// <summary>
    /// Описание разрешения на выполнение действия.
    /// </summary>
    public string? Description { get; private set; }
}
