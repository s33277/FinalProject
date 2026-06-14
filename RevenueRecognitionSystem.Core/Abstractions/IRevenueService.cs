using RevenueRecognitionSystem.Core.DTOs.RevenueDTOs;

namespace RevenueRecognitionSystem.Core.Abstractions;

public interface IRevenueService
{
    Task<RevenueResponse?> GetCurrentRevenueAsync(RevenueCalculationRequest request);
    Task<RevenueResponse?> GetPredictedRevenueAsync(RevenueCalculationRequest request);
    Task<RevenueSummaryResponse?> GetRevenueSummaryAsync(RevenueCalculationRequest request);
}
