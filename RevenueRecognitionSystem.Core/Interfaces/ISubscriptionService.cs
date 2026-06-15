using RevenueRecognitionSystem.Core.DTOs.SubscriptionDTOs;

namespace RevenueRecognitionSystem.Core.Abstractions;

public interface ISubscriptionService
{
    Task<SubscriptionResponse?> CreateAsync(CreateSubscriptionRequest request);
    Task<SubscriptionResponse?> GetByIdAsync(int id);
    Task<List<SubscriptionResponse>> GetAllAsync();
    Task<SubscriptionResponse?> AddPaymentAsync(int subscriptionId, SubscriptionPaymentRequest request);
}
