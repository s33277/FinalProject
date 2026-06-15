using FluentValidation;
using RevenueRecognitionSystem.Core.DTOs.SubscriptionDTOs;

namespace RevenueRecognitionSystem.Core.Validators.SubscriptionValidators;

public class CreateSubscriptionRequestValidator : AbstractValidator<CreateSubscriptionRequest>
{
    public CreateSubscriptionRequestValidator()
    {
        RuleFor(x => x.CustomerId).GreaterThan(0);
        RuleFor(x => x.SoftwareId).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.PricePerPeriod).GreaterThan(0);
        RuleFor(x => x.RenewalPeriodMonths).InclusiveBetween(1, 24);
    }
}
