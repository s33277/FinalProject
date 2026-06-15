using FluentValidation;
using RevenueRecognitionSystem.Core.DTOs.ContractDTOs;

namespace RevenueRecognitionSystem.Core.Validators.ContractValidators;

public class CreateContractRequestValidator : AbstractValidator<CreateContractRequest>
{
    public CreateContractRequestValidator()
    {
        RuleFor(x => x.CustomerId).GreaterThan(0);
        RuleFor(x => x.SoftwareId).GreaterThan(0);
        RuleFor(x => x.SoftwareVersion).MaximumLength(50);
        RuleFor(x => x.AdditionalSupportYears).InclusiveBetween(0, 3);
        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .Must((request, endDate) =>
            {
                var days = (endDate.Date - request.StartDate.Date).TotalDays;
                return days is >= 3 and <= 30;
            })
            .WithMessage("Contract duration must be between 3 and 30 days.");
    }
}
