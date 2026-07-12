using System.Security.Cryptography.X509Certificates;
using FinaccApi.Modules.Categories.Domain;

namespace FinaccApi.Modules.Categories.Application;

public record CategoryResponse(
    int Id,
    string Name,
    CategoryType CategoryType,
    ActivityType ActivityType,
    string? Description,
    int? OrganizationId,
    int? ParentCategoryId
);