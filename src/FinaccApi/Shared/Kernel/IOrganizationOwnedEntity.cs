namespace FinaccApi.Shared.Kernel;

/// <summary>
/// Интерфейс для сущностей, принадлежащих организации.
/// </summary>
public interface IOrganizationOwnedEntity
{
    /// <summary>
    /// Идентификатор организации, которой принадлежит сущность.
    /// </summary>
    int OrganizationId { get; }
}
