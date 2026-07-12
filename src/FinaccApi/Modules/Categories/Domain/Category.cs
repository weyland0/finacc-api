using FinaccApi.Shared.Kernel;

namespace FinaccApi.Modules.Categories.Domain;

/// <summary>
/// Категория финансовой операции: общая или принадлежащая организации.
/// </summary>
public class Category : AuditableEntity
{
    /// <summary>
    /// Название категории.
    /// </summary>
    public string Name { get; private set; } = null!;
    
    /// <summary>
    /// Тип категории: доход или расход.
    /// </summary>
    public CategoryType CategoryType { get; private set; }

    /// <summary>
    /// Вид деятельности: операционная, финансовая или инвестиционная.
    /// </summary>
    public ActivityType ActivityType { get; private set; }

    /// <summary>
    /// Описание категории.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Организация-владелец категории; null означает общую категорию.
    /// </summary>
    public int? OrganizationId { get; private set; }

    /// <summary>
    /// Идентификатор родительской категории.
    /// </summary>
    public int? ParentId { get; private set; }

    /// <summary>
    /// Создает категорию с обязательными данными.
    /// </summary>
    public Category(string name, CategoryType categoryType, ActivityType activityType)
    {
        Name = name;
        CategoryType = categoryType;
        ActivityType = activityType;
    }

    /// <summary>
    /// Создает категорию со всеми доступными данными.
    /// </summary>
    public Category(
        string name,
        CategoryType categoryType,
        ActivityType activityType,
        string? description,
        int? organizationId,
        int? parendId)
    {
        Name = name;
        CategoryType = categoryType;
        ActivityType = activityType;
        Description = description;
        OrganizationId = organizationId;
        ParentId = parendId;
    }
}
