using FluentValidation;
using RevenueRecognitionSystem.Core.DTOs.RevenueDTOs;

namespace RevenueRecognitionSystem.Core.Validators.RevenueValidators;

public class RevenueCalculationRequestValidator : AbstractValidator<RevenueCalculationRequest>
{
    public RevenueCalculationRequestValidator()
    {
        RuleFor(x => x.SoftwareId).GreaterThan(0).When(x => x.SoftwareId.HasValue);
        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3)
            .Matches("^[A-Z]+$")
            .WithMessage("Currency must be a three-letter uppercase currency code.");
    }
}
