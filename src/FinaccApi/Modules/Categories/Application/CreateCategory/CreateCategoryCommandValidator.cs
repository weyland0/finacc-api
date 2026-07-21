using FluentValidation;

namespace FinaccApi.Modules.Categories.Application.CreateCategory;

public sealed class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.CategoryType)
            .IsInEnum();

        RuleFor(x => x.ActivityType)
            .IsInEnum();

        RuleFor(x => x.Description)
            .MaximumLength(1000);
    }   
}
