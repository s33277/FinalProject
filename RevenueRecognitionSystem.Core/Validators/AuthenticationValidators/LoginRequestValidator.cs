using FluentValidation;
using RevenueRecognitionSystem.Core.DTOs.AuthenticationDTOs;

namespace RevenueRecognitionSystem.Core.Validators.AuthenticationValidators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Login).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Password).NotEmpty().MaximumLength(200);
    }
}
