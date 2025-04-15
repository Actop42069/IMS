using FluentValidation;

namespace Application.Vendors.Command
{
    public class UpdateVendorCommandValidator : AbstractValidator<UpdateVendorCommand>
    {
        public UpdateVendorCommandValidator()
        {

            RuleFor(x => x.VendorId)
                .GreaterThan(0)
                .WithMessage("Vendor ID must be greater than 0");

            RuleFor(x => x.VendorName)
                .NotEmpty()
                .WithMessage("Vendor name is required")
                .MaximumLength(100)
                .WithMessage("Vendor name cannot exceed 100 characters");

            RuleFor(x => x.VendorEmail)
                .NotEmpty()
                .WithMessage("Vendor email is required")
                .EmailAddress()
                .WithMessage("Invalid email format")
                .MaximumLength(100)
                .WithMessage("Email cannot exceed 100 characters");

            RuleFor(x => x.VendorPhoneNumber)
                .NotEmpty()
                .WithMessage("Vendor phone number is required")
                .Matches(@"^\+?[0-9\-\(\)\/\s]*$")
                .WithMessage("Invalid phone number format")
                .MaximumLength(20)
                .WithMessage("Phone number cannot exceed 20 characters");

            RuleFor(x => x.VendorAddress)
                .NotEmpty()
                .WithMessage("Vendor address is required")
                .MaximumLength(500)
                .WithMessage("Address cannot exceed 500 characters");

            // Vendor type and status validation
            RuleFor(x => x.VendorType)
                .IsInEnum()
                .WithMessage("Invalid vendor type");

            RuleFor(x => x.VendorStatus)
                .IsInEnum()
                .WithMessage("Invalid vendor status");

            // Updated username validation
            RuleFor(x => x.UpdatedUserName)
                .NotEmpty()
                .WithMessage("Updated username is required")
                .MaximumLength(50)
                .WithMessage("Updated username cannot exceed 50 characters");
        }
    }
}
