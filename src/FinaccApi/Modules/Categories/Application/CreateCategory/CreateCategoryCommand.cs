namespace FinaccApi.Modules.Categories.Application.CreateCategory;

public record CreateCategoryCommand(
    string Name, 
    string CategoryType, 
    string ActivityType,
    string? Description,
    int? ParentCategoryId
);