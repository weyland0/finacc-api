using FinaccApi.Shared.Kernel;

namespace FinaccApi.Modules.Categories.Domain;

/// <summary>
/// Категория финансовой операции.
/// </summary>
public class Category : AuditableEntity
{
	/// <summary>
	/// Название.
	/// </summary>
	public string Name { get; private set; } = null!;

	/// <summary>
	/// Тип.
	/// </summary>
	public CategoryType CategoryType { get; private set; }

	/// <summary>
	/// Вид деятельности.
	/// </summary>
	public ActivityType ActivityType { get; private set; }

	/// <summary>
	/// Описание.
	/// </summary>
	public string? Description { get; private set; }

	/// <summary>
	/// Организация-владелец категории.
	/// </summary>
	public int? OrganizationId { get; private set; }

	/// <summary>
	/// Идентификатор родительской категории.
	/// </summary>
	public int? ParentId { get; private set; }

	/// <summary>
	/// Инициализирует категорию c основными данными.
	/// </summary>
	public Category(string name, CategoryType categoryType, ActivityType activityType)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(name);

		Name = name;
		CategoryType = categoryType;
		ActivityType = activityType;
	}

	/// <summary>
	/// Инициализирует категорию со всеми данными.
	/// </summary>
	public Category(
		string name,
		CategoryType categoryType,
		ActivityType activityType,
		string? description,
		int? organizationId,
		int? parentId)
		: this(name, categoryType, activityType)
	{
		Description = description;
		OrganizationId = organizationId;
		ParentId = parentId;
	}

	/// <summary>
	/// Обновляет название и описание категории.
	/// </summary>
	/// <param name="name"> Название категории. </param>
	/// <param name="description"> Описание категории. </param>
	public void UpdateDetails(string name, string? description)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(name);

		Name = name;
		Description = description;
	}

	/// <summary>
	/// Определяет глобальная ли категория.
	/// </summary>
	/// <returns> Признак глобальности. </returns>
	public bool IsGlobal()
	{
		return OrganizationId == null;
	}
}
