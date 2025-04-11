using FluentValidation;
using System.Data;

namespace Application.Categories.Command
{
    public class DeleteCategoryValidator : AbstractValidator<DeleteCategoryCommand>
    {
        public DeleteCategoryValidator()
        {
            RuleFor(command => command.CategoryId)
               .GreaterThan(0)
               .WithMessage("CategoryId must be greater than 0.");
        }
    }
}
