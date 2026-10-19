using FluentValidation;
using FinaccApi.Modules.Categories.Domain;

namespace FinaccApi.Modules.Categories.Application.UpdateCategory;

/// <summary>
/// Валидатор команды обновления категории.
/// </summary>
public sealed class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
	/// <summary>
	/// Инициализирует валидатор команды обновления.
	/// </summary>
	public UpdateCategoryCommandValidator()
	{
		RuleFor(x => x.CategoryId)
			.GreaterThan(0);

		RuleFor(x => x.Name)
			.NotEmpty()
			.MaximumLength(CategoryConstraints.MaxNameLength);

		RuleFor(x => x.Description)
			.MaximumLength(CategoryConstraints.MaxDescriptionLength);
	}
}
