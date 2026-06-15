using FluentValidation;
using RevenueRecognitionSystem.Core.DTOs.ContractDTOs;

namespace RevenueRecognitionSystem.Core.Validators.ContractValidators;

public class ContractPaymentRequestValidator : AbstractValidator<ContractPaymentRequest>
{
    public ContractPaymentRequestValidator()
    {
        RuleFor(x => x.CustomerId).GreaterThan(0);
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}
