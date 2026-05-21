namespace finacc.Shared.Kernel;


/// <summary>
/// Базовый класс сущности.
/// </summary>
public abstract class Entity
{
    /// <summary>
    /// Идентификатор сущности.
    /// </summary>
    public int Id { get; protected set; }
}
