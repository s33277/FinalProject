using RevenueRecognitionSystem.Core.DTOs.ContractDTOs;

namespace RevenueRecognitionSystem.Core.Abstractions;

public interface IContractService
{
    Task<ContractResponse?> CreateAsync(CreateContractRequest request);
    Task<ContractResponse?> GetByIdAsync(int id);
    Task<List<ContractResponse>> GetAllAsync();
    Task<ContractResponse?> AddPaymentAsync(int contractId, ContractPaymentRequest request);
    Task<bool> DeleteAsync(int id);
}
