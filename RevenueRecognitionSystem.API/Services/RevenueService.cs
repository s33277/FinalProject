using Microsoft.EntityFrameworkCore;
using RevenueRecognitionSystem.Core.Abstractions;
using RevenueRecognitionSystem.Core.DTOs.RevenueDTOs;
using RevenueRecognitionSystem.Data.Context;
using RevenueRecognitionSystem.Data.Entities.Subscriptions;

namespace RevenueRecognitionSystem.API.Services;

public class RevenueService : IRevenueService
{
    private static readonly Dictionary<string, decimal> CurrencyRates = new()
    {
        ["PLN"] = 1m,
        ["EUR"] = 0.23m,
        ["USD"] = 0.25m
    };

    private readonly AppDbContext _dbContext;

    public RevenueService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RevenueResponse?> GetCurrentRevenueAsync(RevenueCalculationRequest request)
    {
        var currency = request.Currency.ToUpperInvariant();
        if (!IsSupportedCurrency(currency))
        {
            return null;
        }

        var amount = await CalculateCurrentRevenueAsync(request.SoftwareId);
        return new RevenueResponse(ConvertCurrency(amount, currency), currency);
    }

    public async Task<RevenueResponse?> GetPredictedRevenueAsync(RevenueCalculationRequest request)
    {
        var currency = request.Currency.ToUpperInvariant();
        if (!IsSupportedCurrency(currency))
        {
            return null;
        }

        var amount = await CalculatePredictedRevenueAsync(request.SoftwareId);
        return new RevenueResponse(ConvertCurrency(amount, currency), currency);
    }

    public async Task<RevenueSummaryResponse?> GetRevenueSummaryAsync(RevenueCalculationRequest request)
    {
        var currency = request.Currency.ToUpperInvariant();
        if (!IsSupportedCurrency(currency))
        {
            return null;
        }

        var currentRevenue = await CalculateCurrentRevenueAsync(request.SoftwareId);
        var predictedRevenue = await CalculatePredictedRevenueAsync(request.SoftwareId);

        return new RevenueSummaryResponse(
            ConvertCurrency(currentRevenue, currency),
            ConvertCurrency(predictedRevenue, currency),
            currency);
    }

    private async Task<decimal> CalculateCurrentRevenueAsync(int? softwareId)
    {
        var signedContracts = _dbContext.Contracts
            .Where(c => c.IsSigned);

        var subscriptionPayments = _dbContext.SubscriptionPayments
            .Include(payment => payment.Subscription)
            .AsQueryable();

        if (softwareId.HasValue)
        {
            signedContracts = signedContracts.Where(c => c.SoftwareId == softwareId.Value);
            subscriptionPayments = subscriptionPayments.Where(payment =>
                payment.Subscription.SoftwareId == softwareId.Value);
        }

        var contractRevenue = await signedContracts.SumAsync(c => (decimal?)c.FinalPrice) ?? 0m;
        var subscriptionRevenue = await subscriptionPayments.SumAsync(payment => (decimal?)payment.Amount) ?? 0m;

        return decimal.Round(contractRevenue + subscriptionRevenue, 2);
    }

    private async Task<decimal> CalculatePredictedRevenueAsync(int? softwareId)
    {
        var currentRevenue = await CalculateCurrentRevenueAsync(softwareId);
        var today = DateTime.UtcNow.Date;

        var unsignedContracts = _dbContext.Contracts
            .Where(c => !c.IsSigned && c.EndDate.Date >= today);

        var activeSubscriptions = _dbContext.Subscriptions
            .Where(s => s.Status == SubscriptionStatus.Active && s.ValidUntil.Date >= today);

        if (softwareId.HasValue)
        {
            unsignedContracts = unsignedContracts.Where(c => c.SoftwareId == softwareId.Value);
            activeSubscriptions = activeSubscriptions.Where(s => s.SoftwareId == softwareId.Value);
        }

        var expectedContractRevenue = await unsignedContracts.SumAsync(c => (decimal?)c.FinalPrice) ?? 0m;
        var expectedSubscriptionRevenue = await activeSubscriptions.SumAsync(s => (decimal?)s.PricePerPeriod) ?? 0m;

        return decimal.Round(currentRevenue + expectedContractRevenue + expectedSubscriptionRevenue, 2);
    }

    private static bool IsSupportedCurrency(string currency)
    {
        return CurrencyRates.ContainsKey(currency);
    }

    private static decimal ConvertCurrency(decimal amount, string currency)
    {
        return decimal.Round(amount * CurrencyRates[currency], 2);
    }
}
