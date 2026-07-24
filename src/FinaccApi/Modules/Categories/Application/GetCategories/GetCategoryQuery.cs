using FinaccApi.Modules.Categories.Domain;

namespace FinaccApi.Modules.Categories.Application.GetCategories;

public record GetCategoriesQuery(
    CategoryType? CategoryType = null,
    ActivityType? ActivityType = null,
    CategoryScope Scope = CategoryScope.AllAvailable,
    string? Search = null,
    bool AsTree = true
);

