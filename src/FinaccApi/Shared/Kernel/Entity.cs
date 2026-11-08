namespace FinaccApi.Shared.Kernel;


/// <summary>
/// Доменная сущность.
/// </summary>
public abstract class Entity
{
    /// <summary>
    /// Уникальный идентификатор сущности.
    /// </summary>
    public Guid Id { get; protected set; } = Guid.CreateVersion7();
}
