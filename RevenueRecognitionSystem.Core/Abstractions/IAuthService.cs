using RevenueRecognitionSystem.Core.DTOs.AuthenticationDTOs;

namespace RevenueRecognitionSystem.Core.Abstractions;

public interface IAuthService
{
    Task<TokenResponse?> LoginAsync(LoginRequest request);
}
