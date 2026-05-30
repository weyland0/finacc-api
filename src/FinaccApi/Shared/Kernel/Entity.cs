namespace FinaccApi.Shared.Kernel;


/// <summary>
/// Базовый класс доменной сущности с числовым идентификатором.
/// </summary>
public abstract class Entity
{
    /// <summary>
    /// Уникальный идентификатор сущности в хранилище данных.
    /// </summary>
    public int Id { get; protected set; }
}
