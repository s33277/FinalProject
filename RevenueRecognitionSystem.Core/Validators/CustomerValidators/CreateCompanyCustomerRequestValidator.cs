using FluentValidation;
using RevenueRecognitionSystem.Core.DTOs.CustomerDTOs;

namespace RevenueRecognitionSystem.Core.Validators.CustomerValidators;

public class CreateCompanyCustomerRequestValidator : AbstractValidator<CreateCompanyCustomerRequest>
{
    public CreateCompanyCustomerRequestValidator()
    {
        RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.KrsNumber)
            .NotEmpty()
            .Length(10)
            .Matches("^[0-9]+$");
        RuleFor(x => x.Address).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(254);
        RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(30);
    }
}
