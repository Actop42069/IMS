using FluentValidation;

namespace Application.Vendors.Command
{
    public class CreateVendorCommandValidator : AbstractValidator<CreateVendorCommand>
    {
        public CreateVendorCommandValidator()
        {
            RuleFor(x => x.VendorName)
                .NotEmpty().WithMessage("Vendor Name is required.")
                .MaximumLength(100).WithMessage("Vendor Name cannot exceed 100 characters.");

            RuleFor(x => x.VendorEmail)
                .NotEmpty().WithMessage("Vendor Email is required.")
                .EmailAddress().WithMessage("Vendor Email must be a valid email address.");

            RuleFor(x => x.VendorPhoneNumber)
                .NotEmpty().WithMessage("Vendor Phone Number is required.")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Vendor Phone Number must be a valid phone number.");

            RuleFor(x => x.VendorAddress)
                .NotEmpty().WithMessage("Vendor Address is required.")
                .MaximumLength(250).WithMessage("Vendor Address cannot exceed 250 characters.");

            RuleFor(x => x.VendorType)
                .IsInEnum().WithMessage("Vendor Type must be a valid enum value.");

            RuleFor(x => x.VendorStatus)
                .IsInEnum().WithMessage("Vendor Status must be a valid enum value.");
        }
    }
}
