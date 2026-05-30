namespace FinaccApi.Shared.Kernel;

/// <summary>
/// Определяет сущность, которая принадлежит конкретной организации.
/// </summary>
public interface IOrganizationOwnedEntity
{
    /// <summary>
    /// Идентификатор организации, которой принадлежит сущность.
    /// </summary>
    int OrganizationId { get; }
}
