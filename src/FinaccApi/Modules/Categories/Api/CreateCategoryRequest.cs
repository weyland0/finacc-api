namespace FinaccApi.Modules.Categories.Api;

public record CreateCategoryRequest(
    string Name, 
    string CategoryType, 
    string ActivityType,
    string? Description,
    int? ParentCategoryId
);
