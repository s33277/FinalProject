using FluentValidation;
using RevenueRecognitionSystem.Core.DTOs.CustomerDTOs;

namespace RevenueRecognitionSystem.Core.Validators.CustomerValidators;

public class CreateIndividualCustomerRequestValidator : AbstractValidator<CreateIndividualCustomerRequest>
{
    public CreateIndividualCustomerRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Pesel)
            .NotEmpty()
            .Length(11)
            .Matches("^[0-9]+$");
        RuleFor(x => x.Address).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(254);
        RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(30);
    }
}
