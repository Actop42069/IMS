using FluentValidation;

namespace Application.Products.Command
{
    public class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0)
                .WithMessage("ProductId must be a positive integer.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .MaximumLength(100)
                .WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description is required.")
                .MaximumLength(500)
                .WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.SKU)
                .NotEmpty()
                .WithMessage("SKU is required.")
                .MaximumLength(50)
                .WithMessage("SKU cannot exceed 50 characters.");

            RuleFor(x => x.ProductStatus)
                .IsInEnum()
                .WithMessage("Invalid product status.");

            RuleFor(x => x.ProductType)
                .IsInEnum()
                .WithMessage("Invalid product type.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Price must be a non-negative value.");

            RuleFor(x => x.UnitCost)
                .GreaterThanOrEqualTo(0)
                .WithMessage("UnitCost must be a non-negative value.");

            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Quantity must be a non-negative integer.");
        }
    }
}
