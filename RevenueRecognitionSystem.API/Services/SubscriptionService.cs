using Microsoft.EntityFrameworkCore;
using RevenueRecognitionSystem.Core.Abstractions;
using RevenueRecognitionSystem.Core.DTOs.SubscriptionDTOs;
using RevenueRecognitionSystem.Data.Context;
using RevenueRecognitionSystem.Data.Entities.Subscriptions;

namespace RevenueRecognitionSystem.API.Services;

public class SubscriptionService : ISubscriptionService
{
    private const decimal LoyalCustomerDiscountPercentage = 5m;

    private readonly AppDbContext _dbContext;

    public SubscriptionService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SubscriptionResponse?> CreateAsync(CreateSubscriptionRequest request)
    {
        var customerExists = await _dbContext.Customers.AnyAsync(c => c.Id == request.CustomerId);
        var softwareExists = await _dbContext.Software.AnyAsync(s => s.Id == request.SoftwareId);

        if (!customerExists || !softwareExists)
        {
            return null;
        }

        var today = DateTime.UtcNow.Date;
        var alreadyHasActiveSubscription = await _dbContext.Subscriptions.AnyAsync(s =>
            s.CustomerId == request.CustomerId &&
            s.SoftwareId == request.SoftwareId &&
            s.Status == SubscriptionStatus.Active &&
            s.ValidUntil.Date >= today);

        var alreadyHasActiveContract = await _dbContext.Contracts.AnyAsync(c =>
            c.CustomerId == request.CustomerId &&
            c.SoftwareId == request.SoftwareId &&
            (c.IsSigned || c.EndDate.Date >= today));

        if (alreadyHasActiveSubscription || alreadyHasActiveContract)
        {
            return null;
        }

        var firstPaymentAmount = await CalculateFirstPaymentAmountAsync(
            request.CustomerId,
            request.PricePerPeriod,
            today);

        var subscription = new Subscription
        {
            CustomerId = request.CustomerId,
            SoftwareId = request.SoftwareId,
            Name = request.Name,
            PricePerPeriod = request.PricePerPeriod,
            RenewalPeriodMonths = request.RenewalPeriodMonths,
            ValidUntil = today.AddMonths(request.RenewalPeriodMonths),
            Status = SubscriptionStatus.Active
        };

        subscription.Payments.Add(new SubscriptionPayment
        {
            Amount = firstPaymentAmount,
            PaymentDate = DateTime.UtcNow,
            CoveredPeriodStart = today,
            CoveredPeriodEnd = subscription.ValidUntil
        });

        _dbContext.Subscriptions.Add(subscription);
        await _dbContext.SaveChangesAsync();

        return MapToResponse(subscription);
    }

    public async Task<SubscriptionResponse?> GetByIdAsync(int id)
    {
        var subscription = await _dbContext.Subscriptions
            .FirstOrDefaultAsync(s => s.Id == id);

        return subscription is null ? null : MapToResponse(subscription);
    }

    public async Task<List<SubscriptionResponse>> GetAllAsync()
    {
        var subscriptions = await _dbContext.Subscriptions.ToListAsync();

        return subscriptions.Select(MapToResponse).ToList();
    }

    public async Task<SubscriptionResponse?> AddPaymentAsync(int subscriptionId, SubscriptionPaymentRequest request)
    {
        var subscription = await _dbContext.Subscriptions
            .Include(s => s.Payments)
            .FirstOrDefaultAsync(s => s.Id == subscriptionId);

        if (subscription is null || subscription.Status != SubscriptionStatus.Active)
        {
            return null;
        }

        var today = DateTime.UtcNow.Date;
        if (today > subscription.ValidUntil.Date)
        {
            subscription.Status = SubscriptionStatus.Cancelled;
            await _dbContext.SaveChangesAsync();
            return null;
        }

        var currentPeriodAlreadyPaid = subscription.Payments.Any(payment =>
            payment.CoveredPeriodStart.Date <= today &&
            payment.CoveredPeriodEnd.Date > today);

        if (currentPeriodAlreadyPaid)
        {
            return null;
        }

        var expectedAmount = decimal.Round(
            subscription.PricePerPeriod * (1 - LoyalCustomerDiscountPercentage / 100m),
            2);

        if (request.Amount != expectedAmount)
        {
            return null;
        }

        var periodStart = subscription.ValidUntil.Date;
        var periodEnd = periodStart.AddMonths(subscription.RenewalPeriodMonths);

        subscription.Payments.Add(new SubscriptionPayment
        {
            Amount = request.Amount,
            PaymentDate = DateTime.UtcNow,
            CoveredPeriodStart = periodStart,
            CoveredPeriodEnd = periodEnd
        });
        subscription.ValidUntil = periodEnd;

        await _dbContext.SaveChangesAsync();

        return MapToResponse(subscription);
    }

    private async Task<decimal> CalculateFirstPaymentAmountAsync(
        int customerId,
        decimal pricePerPeriod,
        DateTime purchaseDate)
    {
        var highestDiscount = await _dbContext.Discounts
            .Where(d => d.OfferType == "Subscription" &&
                        d.ValidFrom.Date <= purchaseDate.Date &&
                        d.ValidTo.Date >= purchaseDate.Date)
            .MaxAsync(d => (decimal?)d.ValuePercentage) ?? 0m;

        var hasPreviousPurchase = await _dbContext.Contracts.AnyAsync(c =>
                                      c.CustomerId == customerId && c.IsSigned) ||
                                  await _dbContext.Subscriptions.AnyAsync(s =>
                                      s.CustomerId == customerId);

        var price = pricePerPeriod * (1 - highestDiscount / 100m);

        if (hasPreviousPurchase)
        {
            price *= 1 - LoyalCustomerDiscountPercentage / 100m;
        }

        return decimal.Round(price, 2);
    }

    private static SubscriptionResponse MapToResponse(Subscription subscription)
    {
        return new SubscriptionResponse(
            subscription.Id,
            subscription.CustomerId,
            subscription.SoftwareId,
            subscription.Name,
            subscription.PricePerPeriod,
            subscription.RenewalPeriodMonths,
            subscription.ValidUntil,
            subscription.Status.ToString());
    }
}
