using Microsoft.EntityFrameworkCore;
using RevenueRecognitionSystem.Core.Abstractions;
using RevenueRecognitionSystem.Core.DTOs.CustomerDTOs;
using RevenueRecognitionSystem.Data.Context;
using RevenueRecognitionSystem.Data.Entities.Customers;

namespace RevenueRecognitionSystem.API.Services;

public class CustomerService : ICustomerService
{
    private readonly AppDbContext _dbContext;

    public CustomerService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CustomerResponse?> CreateIndividualAsync(CreateIndividualCustomerRequest request)
    {
        var peselExists = await _dbContext.Customers
            .OfType<IndividualCustomer>()
            .AnyAsync(c => c.Pesel == request.Pesel);

        if (peselExists)
        {
            return null;
        }

        var customer = new IndividualCustomer
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Pesel = request.Pesel,
            Address = request.Address,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber
        };

        _dbContext.Customers.Add(customer);
        await _dbContext.SaveChangesAsync();

        return MapToResponse(customer);
    }

    public async Task<CustomerResponse?> CreateCompanyAsync(CreateCompanyCustomerRequest request)
    {
        var krsExists = await _dbContext.Customers
            .OfType<CompanyCustomer>()
            .AnyAsync(c => c.KrsNumber == request.KrsNumber);

        if (krsExists)
        {
            return null;
        }

        var customer = new CompanyCustomer
        {
            CompanyName = request.CompanyName,
            KrsNumber = request.KrsNumber,
            Address = request.Address,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber
        };

        _dbContext.Customers.Add(customer);
        await _dbContext.SaveChangesAsync();

        return MapToResponse(customer);
    }

    public async Task<CustomerResponse?> GetByIdAsync(int id)
    {
        var customer = await _dbContext.Customers
            .FirstOrDefaultAsync(c => c.Id == id);

        if (customer is null)
        {
            return null;
        }

        return MapToResponse(customer);
    }

    public async Task<List<CustomerResponse>> GetAllAsync()
    {
        var customers = await _dbContext.Customers.ToListAsync();

        return customers
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<CustomerResponse?> UpdateAsync(int id, UpdateCustomerRequest request)
    {
        var customer = await _dbContext.Customers
            .FirstOrDefaultAsync(c => c.Id == id);

        if (customer is null)
        {
            return null;
        }

        customer.Address = request.Address;
        customer.Email = request.Email;
        customer.PhoneNumber = request.PhoneNumber;

        if (customer is IndividualCustomer individualCustomer)
        {
            if (request.FirstName is not null)
            {
                individualCustomer.FirstName = request.FirstName;
            }

            if (request.LastName is not null)
            {
                individualCustomer.LastName = request.LastName;
            }
        }
        else if (customer is CompanyCustomer companyCustomer)
        {
            if (request.CompanyName is not null)
            {
                companyCustomer.CompanyName = request.CompanyName;
            }
        }

        await _dbContext.SaveChangesAsync();

        return MapToResponse(customer);
    }

    public async Task<bool> DeleteIndividualAsync(int id)
    {
        var customer = await _dbContext.Customers
            .FirstOrDefaultAsync(c => c.Id == id);

        if (customer is not IndividualCustomer individualCustomer)
        {
            return false;
        }

        individualCustomer.FirstName = "Deleted";
        individualCustomer.LastName = "Deleted";
        individualCustomer.Address = "Deleted";
        individualCustomer.Email = $"deleted-customer-{individualCustomer.Id}@deleted.local";
        individualCustomer.PhoneNumber = "Deleted";
        individualCustomer.IsDeleted = true;

        await _dbContext.SaveChangesAsync();

        return true;
    }

    private static CustomerResponse MapToResponse(Customer customer)
    {
        if (customer is IndividualCustomer individualCustomer)
        {
            return new CustomerResponse(
                individualCustomer.Id,
                "Individual",
                $"{individualCustomer.FirstName} {individualCustomer.LastName}",
                individualCustomer.Address,
                individualCustomer.Email,
                individualCustomer.PhoneNumber);
        }

        if (customer is CompanyCustomer companyCustomer)
        {
            return new CustomerResponse(
                companyCustomer.Id,
                "Company",
                companyCustomer.CompanyName,
                companyCustomer.Address,
                companyCustomer.Email,
                companyCustomer.PhoneNumber);
        }

        return new CustomerResponse(
            customer.Id,
            "Unknown",
            "Unknown",
            customer.Address,
            customer.Email,
            customer.PhoneNumber);
    }
}
