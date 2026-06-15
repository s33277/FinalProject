using FluentValidation;
using RevenueRecognitionSystem.Core.DTOs.SubscriptionDTOs;

namespace RevenueRecognitionSystem.Core.Validators.SubscriptionValidators;

public class SubscriptionPaymentRequestValidator : AbstractValidator<SubscriptionPaymentRequest>
{
    public SubscriptionPaymentRequestValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}
