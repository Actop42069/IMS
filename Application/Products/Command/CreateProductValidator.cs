using FluentValidation;

namespace Application.Products.Command
{
    public class CreateProductValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductValidator()
        {
            RuleFor(x => x.VendorId)
               .GreaterThan(0).WithMessage("VendorId must be greater than 0.");


            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("CategoryId must be greater than 0.");


            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product Name is required.")
                .Length(2, 100).WithMessage("Product Name must be between 2 and 100 characters.");


            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Product Description is required.")
                .Length(10, 500).WithMessage("Product Description must be between 10 and 500 characters.");

            RuleFor(x => x.SKU)
                .NotEmpty().WithMessage("SKU is required.")
                .Length(2, 50).WithMessage("SKU must be between 2 and 50 characters.");


            RuleFor(x => x.ProductStatus)
                .IsInEnum().WithMessage("Invalid Product Status.");


            RuleFor(x => x.ProductType)
                .IsInEnum().WithMessage("Invalid Product Type.");


            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.");


            RuleFor(x => x.UnitCost)
                .GreaterThan(0).WithMessage("UnitCost must be greater than 0.");

            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0).WithMessage("Quantity must be 0 or greater.");
        }
    }
}
