using Application.Customers.Command;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Application.Customers.Validator
{
    public class CreateCustomerValidator : AbstractValidator<CreateCustomerCommand>
    {
        public CreateCustomerValidator()
        {

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .Length(2, 50).WithMessage("First name must be between 2 and 50 characters.");


            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .Length(2, 50).WithMessage("Last name must be between 2 and 50 characters.");

   
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(100).WithMessage("Email cannot be longer than 100 characters.");

   
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?\d{10,15}$").WithMessage("Phone number must be between 10 and 15 digits, and can optionally start with '+'.");

     
            RuleFor(x => x.CustomerType)
                .IsInEnum().WithMessage("Invalid customer type.");

        
            RuleFor(x => x.Image)
                .Must(image => image == null || (image.Length > 0 && IsValidImageExtension(image)))
                .WithMessage("Invalid image file. Ensure the file is provided and has a valid extension.");
        }
        private bool IsValidImageExtension(IFormFile image)
        {
            var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(image.FileName).ToLower();
            return validExtensions.Contains(extension);
        }
    }
}
