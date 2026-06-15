using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RevenueRecognitionSystem.API.Services;
using RevenueRecognitionSystem.Core.DTOs.ContractDTOs;
using RevenueRecognitionSystem.Core.DTOs.RevenueDTOs;
using RevenueRecognitionSystem.Data.Context;
using RevenueRecognitionSystem.Data.Entities.Contracts;
using RevenueRecognitionSystem.Data.Entities.Customers;
using RevenueRecognitionSystem.Data.Entities.SoftwareProducts;

namespace RevenueRecognitionSystem.UnitTests;

public class BusinessLogicTests
{
    [Fact]
    public async Task DeleteIndividualAsync_SoftDeletesCustomer()
    {
        await using var dbContext = CreateDbContext();
        var customer = new IndividualCustomer
        {
            FirstName = "Jan",
            LastName = "Kowalski",
            Pesel = "90010112345",
            Address = "Street 1",
            Email = "jan@test.pl",
            PhoneNumber = "123456789"
        };
        dbContext.Customers.Add(customer);
        await dbContext.SaveChangesAsync();

        var service = new CustomerService(dbContext);

        var result = await service.DeleteIndividualAsync(customer.Id);

        result.Should().BeTrue();

        var deletedCustomer = await dbContext.Customers
            .IgnoreQueryFilters()
            .OfType<IndividualCustomer>()
            .SingleAsync(c => c.Id == customer.Id);

        deletedCustomer.IsDeleted.Should().BeTrue();
        deletedCustomer.FirstName.Should().Be("Deleted");
    }

    [Fact]
    public async Task AddPaymentAsync_WhenFullAmountIsPaid_SignsContract()
    {
        await using var dbContext = CreateDbContext();
        var customer = AddCustomer(dbContext);
        var software = AddSoftware(dbContext);
        var contract = new Contract
        {
            Customer = customer,
            Software = software,
            SoftwareVersion = "1.0",
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddDays(7),
            OriginalPrice = 1000m,
            FinalPrice = 1000m,
            AdditionalSupportYears = 0,
            IsSigned = false
        };
        dbContext.Contracts.Add(contract);
        await dbContext.SaveChangesAsync();

        var service = new ContractService(dbContext);

        var response = await service.AddPaymentAsync(contract.Id, new ContractPaymentRequest(customer.Id, 1000m));

        response.Should().NotBeNull();
        response!.IsSigned.Should().BeTrue();
    }

    [Fact]
    public async Task AddPaymentAsync_WhenContractDeadlinePassed_RejectsPaymentAndRefundsOldPayments()
    {
        await using var dbContext = CreateDbContext();
        var customer = AddCustomer(dbContext);
        var software = AddSoftware(dbContext);
        var contract = new Contract
        {
            Customer = customer,
            Software = software,
            SoftwareVersion = "1.0",
            StartDate = DateTime.UtcNow.Date.AddDays(-10),
            EndDate = DateTime.UtcNow.Date.AddDays(-1),
            OriginalPrice = 1000m,
            FinalPrice = 1000m,
            AdditionalSupportYears = 0,
            IsSigned = false,
            Payments =
            {
                new ContractPayment
                {
                    Amount = 200m,
                    PaymentDate = DateTime.UtcNow.AddDays(-2),
                    IsRefunded = false
                }
            }
        };
        dbContext.Contracts.Add(contract);
        await dbContext.SaveChangesAsync();

        var service = new ContractService(dbContext);

        var response = await service.AddPaymentAsync(contract.Id, new ContractPaymentRequest(customer.Id, 800m));

        response.Should().BeNull();
        var payment = await dbContext.ContractPayments.SingleAsync();
        payment.IsRefunded.Should().BeTrue();
    }

    [Fact]
    public async Task GetCurrentRevenueAsync_CountsOnlySignedContracts()
    {
        await using var dbContext = CreateDbContext();
        var customer = AddCustomer(dbContext);
        var software = AddSoftware(dbContext);
        dbContext.Contracts.AddRange(
            new Contract
            {
                Customer = customer,
                Software = software,
                SoftwareVersion = "1.0",
                StartDate = DateTime.UtcNow.Date,
                EndDate = DateTime.UtcNow.Date.AddDays(7),
                OriginalPrice = 1000m,
                FinalPrice = 1000m,
                AdditionalSupportYears = 0,
                IsSigned = true
            },
            new Contract
            {
                Customer = customer,
                Software = software,
                SoftwareVersion = "1.0",
                StartDate = DateTime.UtcNow.Date,
                EndDate = DateTime.UtcNow.Date.AddDays(7),
                OriginalPrice = 5000m,
                FinalPrice = 5000m,
                AdditionalSupportYears = 0,
                IsSigned = false
            });
        await dbContext.SaveChangesAsync();

        var service = new RevenueService(dbContext);

        var response = await service.GetCurrentRevenueAsync(new RevenueCalculationRequest(null, "PLN"));

        response.Should().NotBeNull();
        response!.Amount.Should().Be(1000m);
    }

    [Fact]
    public async Task GetCurrentRevenueAsync_ConvertsRevenueToUsd()
    {
        await using var dbContext = CreateDbContext();
        var customer = AddCustomer(dbContext);
        var software = AddSoftware(dbContext);
        dbContext.Contracts.Add(new Contract
        {
            Customer = customer,
            Software = software,
            SoftwareVersion = "1.0",
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddDays(7),
            OriginalPrice = 1000m,
            FinalPrice = 1000m,
            AdditionalSupportYears = 0,
            IsSigned = true
        });
        await dbContext.SaveChangesAsync();

        var service = new RevenueService(dbContext);

        var response = await service.GetCurrentRevenueAsync(new RevenueCalculationRequest(null, "USD"));

        response.Should().NotBeNull();
        response!.Amount.Should().Be(250m);
        response.Currency.Should().Be("USD");
    }

    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static IndividualCustomer AddCustomer(AppDbContext dbContext)
    {
        var customer = new IndividualCustomer
        {
            FirstName = "Anna",
            LastName = "Nowak",
            Pesel = Guid.NewGuid().ToString("N")[..11],
            Address = "Test Street",
            Email = "anna@test.pl",
            PhoneNumber = "123123123"
        };

        dbContext.Customers.Add(customer);
        return customer;
    }

    private static Software AddSoftware(AppDbContext dbContext)
    {
        var software = new Software
        {
            Name = "Test Software",
            Description = "Test",
            CurrentVersion = "1.0",
            Category = "Finance",
            BasePriceOneYear = 1000m,
            BasePriceSubscription = 100m
        };

        dbContext.Software.Add(software);
        return software;
    }
}
