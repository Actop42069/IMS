using FluentValidation;

namespace Application.Vendors.Command
{
    public class DeleteVendorValidator : AbstractValidator<DeleteVendorCommand>
    {
        public DeleteVendorValidator()
        {
            RuleFor(x => x.VendorId).NotEmpty()
                .WithMessage("Please enter VendorId");
        }
    }
}
