using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinaccApi.Modules.Categories.Application.CreateCategory;
using FinaccApi.Shared.Infrastructure;

namespace FinaccApi.Modules.Categories.Api;

[ApiController]
[Route("api/categories")]
[Authorize]
public class CategoriesController
{
    private readonly CreateCategoryHandler _createCategoryHandler;

    public CategoriesController(CreateCategoryHandler createCategoryHandler)
    {
        _createCategoryHandler = createCategoryHandler;
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

}
