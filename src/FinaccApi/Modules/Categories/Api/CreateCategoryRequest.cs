using FinaccApi.Modules.Categories.Domain;

namespace FinaccApi.Modules.Categories.Api;

public record CreateCategoryRequest(
    string Name, 
    CategoryType CategoryType, 
    ActivityType ActivityType,
    string? Description,
    int? ParentCategoryId
);
