using System.Security.Cryptography.X509Certificates;

namespace FinaccApi.Modules.Categories.Application;

public record CategoryResponse(
    int Id,
    string Name,
    string CategoryType,
    string ActivityType,
    string? Description,
    int? OrganizationId,
    int? ParentCategoryId
);