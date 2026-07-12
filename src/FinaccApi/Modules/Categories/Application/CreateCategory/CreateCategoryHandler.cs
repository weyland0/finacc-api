using Microsoft.EntityFrameworkCore;
using FinaccApi.DataAccess;
using FinaccApi.Shared.Kernel;
using FinaccApi.Shared.Application.Auth;
using FinaccApi.Shared.Application.MultiTenancy;
using FinaccApi.Modules.Access.Domain;
using FinaccApi.Modules.Categories.Domain;

namespace FinaccApi.Modules.Categories.Application.CreateCategory;

public class CreateCategoryHandler
{
    ApplicationDbContext _dbContext;
    ICurrentUser _currentUser;
    ICurrentOrganization _currentOrganization; 
    IPermissionChecker _permissionChecker;

    public CreateCategoryHandler(
        ApplicationDbContext dbContext,
        ICurrentUser currentUser,
        ICurrentOrganization currentOrganization,
        IPermissionChecker permissionChecker
        )
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
        _currentOrganization = currentOrganization;
        _permissionChecker = permissionChecker;
    }

    public async Task<Result<CategoryResponse>> Handle(
        CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        await _currentOrganization.LoadContext(cancellationToken);

        if (!_currentOrganization.HasOrganization)
        {
            return Result.Failure<CategoryResponse>(CategoryErrors.OrganizationNotFound);
        }

        // проверка есть ли доступ на выполнение этой операции у пользователя,
        // который отправил запрос
        var permissionCheckResult = await _permissionChecker.HasPermission(
            Permissions.Categories.Create, cancellationToken);

        if (!permissionCheckResult)
        {
            return Result.Failure<CategoryResponse>(CategoryErrors.Forbidden);
        }

        // если в запросе указана родительская категория, то надо проверить 
        // существует ли она, является доступной для текущей организации
        // и совместима по типу (доход/расход) и по типу активности.
        if (command.ParentCategoryId != null)
        {
            var parentCategory = await _dbContext.Categories.FirstOrDefaultAsync(c =>
                (c.Id == command.ParentCategoryId) &&
                (c.OrganizationId == null || c.OrganizationId != _currentOrganization.OrganizationId),
                cancellationToken);

            if (parentCategory == null)
            {
                return Result.Failure<CategoryResponse>(CategoryErrors.ParentCategoryNotFound);
            }

            if (parentCategory.CategoryType != command.CategoryType ||
                parentCategory.ActivityType != command.ActivityType)
            {
                return Result.Failure<CategoryResponse>(CategoryErrors.ParentCategoryTypeConflict); 
            }
        }

        var newCategory = new Category(
            command.Name,
            command.CategoryType,
            command.ActivityType,
            command.Description,
            _currentOrganization.OrganizationId,
            command.ParentCategoryId);

        newCategory.MarkCreated(_currentUser.RequiredUserId);
            
        _dbContext.Categories.Add(newCategory);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(newCategory));
    }

    private static CategoryResponse MapToResponse(Category category)
    {
        return new CategoryResponse(
            category.Id,
            category.Name,
            category.CategoryType,
            category.ActivityType,
            category.Description,
            category.OrganizationId,
            category.ParentId
        );
    }
}
