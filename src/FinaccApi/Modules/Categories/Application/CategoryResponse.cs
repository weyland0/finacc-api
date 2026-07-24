using FinaccApi.Modules.Categories.Domain;

namespace FinaccApi.Modules.Categories.Application;

public record CategoryResponse(
    int Id,
    string Name,
    CategoryType CategoryType,
    ActivityType ActivityType,
    string? Description,
    int? OrganizationId,
    int? ParentCategoryId,
    bool IsGlobal,
    bool CanEdit,
    IReadOnlyList<CategoryResponse> Children
);