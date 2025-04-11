using FluentValidation;
using System.CodeDom;

namespace Application.Products.Command
{
    public class DeleteProductValidator : AbstractValidator<DeleteProductCommand>
    {
        public DeleteProductValidator()
        {
            RuleFor(command => command.ProductId)
               .GreaterThan(0)
               .WithMessage("ProductId must be greater than 0.");
        }
    }
}
