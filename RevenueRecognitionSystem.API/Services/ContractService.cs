using Microsoft.EntityFrameworkCore;
using RevenueRecognitionSystem.Core.Abstractions;
using RevenueRecognitionSystem.Core.DTOs.ContractDTOs;
using RevenueRecognitionSystem.Data.Context;
using RevenueRecognitionSystem.Data.Entities.Contracts;
using RevenueRecognitionSystem.Data.Entities.Subscriptions;

namespace RevenueRecognitionSystem.API.Services;

public class ContractService : IContractService
{
    private const decimal AdditionalSupportYearPrice = 1000m;
    private const decimal ReturningCustomerDiscountPercentage = 5m;

    private readonly AppDbContext _dbContext;

    public ContractService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ContractResponse?> CreateAsync(CreateContractRequest request)
    {
        var customerExists = await _dbContext.Customers.AnyAsync(c => c.Id == request.CustomerId);
        var software = await _dbContext.Software.FirstOrDefaultAsync(s => s.Id == request.SoftwareId);

        if (!customerExists || software is null)
        {
            return null;
        }

        var today = DateTime.UtcNow.Date;
        var hasActiveContract = await _dbContext.Contracts.AnyAsync(c =>
            c.CustomerId == request.CustomerId &&
            c.SoftwareId == request.SoftwareId &&
            (c.IsSigned || c.EndDate.Date >= today));

        var hasActiveSubscription = await _dbContext.Subscriptions.AnyAsync(s =>
            s.CustomerId == request.CustomerId &&
            s.SoftwareId == request.SoftwareId &&
            s.Status == SubscriptionStatus.Active &&
            s.ValidUntil.Date >= today);

        if (hasActiveContract || hasActiveSubscription)
        {
            return null;
        }

        var originalPrice = software.BasePriceOneYear +
                            request.AdditionalSupportYears * AdditionalSupportYearPrice;
        var finalPrice = await ApplyContractDiscountsAsync(request.CustomerId, originalPrice, request.StartDate);

        var contract = new Contract
        {
            CustomerId = request.CustomerId,
            SoftwareId = request.SoftwareId,
            SoftwareVersion = request.SoftwareVersion ?? software.CurrentVersion,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            AdditionalSupportYears = request.AdditionalSupportYears,
            OriginalPrice = originalPrice,
            FinalPrice = finalPrice,
            IsSigned = false
        };

        _dbContext.Contracts.Add(contract);
        await _dbContext.SaveChangesAsync();

        return MapToResponse(contract, 0m);
    }

    public async Task<ContractResponse?> GetByIdAsync(int id)
    {
        var contract = await _dbContext.Contracts
            .Include(c => c.Payments)
            .FirstOrDefaultAsync(c => c.Id == id);

        return contract is null ? null : MapToResponse(contract);
    }

    public async Task<List<ContractResponse>> GetAllAsync()
    {
        var contracts = await _dbContext.Contracts
            .Include(c => c.Payments)
            .ToListAsync();

        return contracts.Select(contract => MapToResponse(contract)).ToList();
    }

    public async Task<ContractResponse?> AddPaymentAsync(int contractId, ContractPaymentRequest request)
    {
        var contract = await _dbContext.Contracts
            .Include(c => c.Payments)
            .FirstOrDefaultAsync(c => c.Id == contractId);

        if (contract is null || contract.CustomerId != request.CustomerId || contract.IsSigned)
        {
            return null;
        }

        var today = DateTime.UtcNow.Date;
        if (today > contract.EndDate.Date)
        {
            foreach (var payment in contract.Payments)
            {
                payment.IsRefunded = true;
            }

            await _dbContext.SaveChangesAsync();
            return null;
        }

        var amountPaid = GetAmountPaid(contract);
        if (amountPaid + request.Amount > contract.FinalPrice)
        {
            return null;
        }

        var paymentToAdd = new ContractPayment
        {
            ContractId = contract.Id,
            Amount = request.Amount,
            PaymentDate = DateTime.UtcNow,
            IsRefunded = false
        };

        _dbContext.ContractPayments.Add(paymentToAdd);
        amountPaid += request.Amount;

        if (amountPaid == contract.FinalPrice)
        {
            contract.IsSigned = true;
        }

        await _dbContext.SaveChangesAsync();

        return MapToResponse(contract, amountPaid);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var contract = await _dbContext.Contracts.FirstOrDefaultAsync(c => c.Id == id);

        if (contract is null)
        {
            return false;
        }

        _dbContext.Contracts.Remove(contract);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    private async Task<decimal> ApplyContractDiscountsAsync(int customerId, decimal originalPrice, DateTime contractDate)
    {
        var promotionalDiscount = await GetHighestDiscountPercentageAsync("Software", contractDate);
        var hasPreviousPurchase = await IsReturningCustomerAsync(customerId);

        var price = ApplyPercentageDiscount(originalPrice, promotionalDiscount);

        if (hasPreviousPurchase)
        {
            price = ApplyPercentageDiscount(price, ReturningCustomerDiscountPercentage);
        }

        return decimal.Round(price, 2);
    }

    private async Task<decimal> GetHighestDiscountPercentageAsync(string offerType, DateTime date)
    {
        var acceptedOfferTypes = offerType == "Software"
            ? new[] { "Software", "Contract", "Purchase" }
            : new[] { offerType };

        return await _dbContext.Discounts
            .Where(d => acceptedOfferTypes.AsEnumerable().Contains(d.OfferType) &&
                        d.ValidFrom.Date <= date.Date &&
                        d.ValidTo.Date >= date.Date)
            .MaxAsync(d => (decimal?)d.ValuePercentage) ?? 0m;
    }

    private async Task<bool> IsReturningCustomerAsync(int customerId)
    {
        var hasSignedContract = await _dbContext.Contracts.AnyAsync(c =>
            c.CustomerId == customerId && c.IsSigned);
        var hasSubscription = await _dbContext.Subscriptions.AnyAsync(s =>
            s.CustomerId == customerId);

        return hasSignedContract || hasSubscription;
    }

    private static decimal ApplyPercentageDiscount(decimal price, decimal percentage)
    {
        return price * (1 - percentage / 100m);
    }

    private static decimal GetAmountPaid(Contract contract)
    {
        return contract.Payments
            .Where(payment => !payment.IsRefunded)
            .Sum(payment => payment.Amount);
    }

    private static ContractResponse MapToResponse(Contract contract)
    {
        return MapToResponse(contract, GetAmountPaid(contract));
    }

    private static ContractResponse MapToResponse(Contract contract, decimal amountPaid)
    {
        return new ContractResponse(
            contract.Id,
            contract.CustomerId,
            contract.SoftwareId,
            contract.SoftwareVersion,
            contract.StartDate,
            contract.EndDate,
            contract.OriginalPrice,
            contract.FinalPrice,
            amountPaid,
            contract.IsSigned);
    }
}
