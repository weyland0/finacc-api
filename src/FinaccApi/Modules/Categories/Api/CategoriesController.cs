using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinaccApi.Shared.Infrastructure;
using FinaccApi.Modules.Categories.Domain;
using FinaccApi.Modules.Categories.Application.CreateCategory;
using FinaccApi.Modules.Categories.Application.GetCategories;

namespace FinaccApi.Modules.Categories.Api;

[ApiController]
[Route("api/categories")]
[Authorize]
public class CategoriesController
{
    private readonly CreateCategoryHandler _createCategoryHandler;
    private readonly GetCategoriesHandler _getCategoriesHandler;

    public CategoriesController(
        CreateCategoryHandler createCategoryHandler,
        GetCategoriesHandler getCategoriesHandler)
    {
        _createCategoryHandler = createCategoryHandler;
        _getCategoriesHandler = getCategoriesHandler;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(
        [FromBody] CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateCategoryCommand(
            request.Name,
            request.CategoryType,
            request.ActivityType,
            request.Description,
            request.ParentCategoryId);

        var result = await _createCategoryHandler.Handle(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] CategoryType? categoryType,
        [FromQuery] ActivityType? activityType,
        [FromQuery] CategoryScope scope = CategoryScope.AllAvailable,
        [FromQuery] string? search = null,
        [FromQuery] bool asTree = true,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCategoriesQuery(
            categoryType,
            activityType,
            scope,
            search,
            asTree
        );

        var result = await _getCategoriesHandler.Handle(query, cancellationToken);
        return result.ToActionResult();
    }

}
