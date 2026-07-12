using FinaccApi.Modules.Categories.Domain;

namespace FinaccApi.Modules.Categories.Application.CreateCategory;

public record CreateCategoryCommand(
    string Name, 
    CategoryType CategoryType, 
    ActivityType ActivityType,
    string? Description,
    int? ParentCategoryId
);