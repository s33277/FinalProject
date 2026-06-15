using RevenueRecognitionSystem.Core.DTOs.CustomerDTOs;

namespace RevenueRecognitionSystem.Core.Abstractions;

public interface ICustomerService
{
    Task<CustomerResponse?> CreateIndividualAsync(CreateIndividualCustomerRequest request);
    Task<CustomerResponse?> CreateCompanyAsync(CreateCompanyCustomerRequest request);
    Task<CustomerResponse?> GetByIdAsync(int id);
    Task<List<CustomerResponse>> GetAllAsync();
    Task<CustomerResponse?> UpdateAsync(int id, UpdateCustomerRequest request);
    Task<bool> DeleteIndividualAsync(int id);
}
