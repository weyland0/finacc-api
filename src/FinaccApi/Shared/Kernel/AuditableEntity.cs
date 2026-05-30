namespace FinaccApi.Shared.Kernel;

/// <summary>
/// Базовый класс доменной сущности с полями аудита создания и изменения.
/// </summary>
public abstract class AuditableEntity : Entity
{
    /// <summary>
    /// Дата и время создания сущности в UTC.
    /// </summary>
    public DateTime CreatedAt { get; protected set; }

    /// <summary>
    /// Дата и время последнего обновления сущности в UTC.
    /// </summary>
    public DateTime UpdatedAt { get; protected set; }

    /// <summary>
    /// Идентификатор пользователя, создавшего сущность, если он известен.
    /// </summary>
    public int? CreatedBy { get; protected set; }

    /// <summary>
    /// Идентификатор пользователя, последним обновившего сущность, если он известен.
    /// </summary>
    public int? UpdatedBy { get; protected set; }


    /// <summary>
    /// Заполняет поля аудита при создании сущности.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя, создавшего сущность, или null для системного действия.</param>
    public void MarkCreated(int? userId)
    {
        CreatedBy = userId;
        UpdatedBy = userId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Обновляет поля аудита при изменении сущности.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя, обновившего сущность, или null для системного действия.</param>
    public void MarkUpdated(int? userId)
    {
        UpdatedBy = userId;
        UpdatedAt = DateTime.UtcNow;
    }
}
