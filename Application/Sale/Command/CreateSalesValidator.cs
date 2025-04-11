using Domain.Entities;
using FluentValidation;

namespace Application.Sale.Command
{
    public class CreateSalesValidator : AbstractValidator<Sales>
    {
        public CreateSalesValidator()
        {
            RuleFor(x => x.PaymentMethod)
                .IsInEnum().WithMessage("PaymentMethod is not valid.");

            RuleFor(x => x.TranscationId)
                .NotEmpty().WithMessage("TranscationId cannot be empty.")
                .Matches(@"^[a-zA-Z0-9]*$").WithMessage("TranscationId must contain only alphanumeric characters.");

            RuleFor(x => x.PaymentStatus)
                .IsInEnum().WithMessage("PaymentStatus is not valid.");
        }
    }
}
