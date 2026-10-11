using FluentValidation;
using FinaccApi.Modules.Categories.Domain;

namespace FinaccApi.Modules.Categories.Application.CreateCategory;

/// <summary>
/// Валидатор команды создания категории.
/// </summary>
public sealed class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
	/// <summary>
	/// Инициализирует валидатор.
	/// </summary>
	public CreateCategoryCommandValidator()
	{
		RuleFor(x => x.Name)
			.NotEmpty()
			.MaximumLength(CategoryConstraints.MaxNameLength);

		RuleFor(x => x.CategoryType)
			.IsInEnum();

		RuleFor(x => x.ActivityType)
			.IsInEnum();

		RuleFor(x => x.Description)
			.MaximumLength(CategoryConstraints.MaxDescriptionLength);
	}
}
