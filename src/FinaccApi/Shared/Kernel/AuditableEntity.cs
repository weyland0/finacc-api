namespace finacc.Shared.Kernel;

/// <summary>
/// Базовый класс сущности с аудитом.
/// </summary>
public abstract class AuditableEntity : Entity
{
    /// <summary>
    /// Дата создания сущности.
    /// </summary>
    public DateTime CreatedAt { get; protected set; }

    /// <summary>
    /// Дата обновления сущности.
    /// </summary>
    public DateTime UpdatedAt { get; protected set; }

    /// <summary>
    /// Идентификатор пользователя, создавшего сущность.
    /// </summary>
    public int? CreatedBy { get; protected set; }

    /// <summary>
    /// Идентификатор пользователя, обновившего сущность.
    /// </summary>
    public int? UpdatedBy { get; protected set; }


    /// <summary>
    /// Отметить сущность как созданную.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя, создавшего сущность.</param>
    public void MarkCreated(int? userId)
    {
        CreatedBy = userId;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Отметить сущность как обновленную.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя, обновившего сущность.</param>
    public void MarkUpdated(int? userId)
    {
        UpdatedBy = userId;
        UpdatedAt = DateTime.UtcNow;
    }
}

