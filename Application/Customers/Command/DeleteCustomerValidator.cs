using FluentValidation;
using Application.Customers.Command;

namespace Application.Customers.Validator
{
    public class DeleteCustomerValidator : AbstractValidator<DeleteCustomerCommand>
    {
        public DeleteCustomerValidator()
        {
            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithMessage("CustomerId must be greater than 0.")
                .NotEmpty().WithMessage("CustomerId is required.");
        }
    }
}
