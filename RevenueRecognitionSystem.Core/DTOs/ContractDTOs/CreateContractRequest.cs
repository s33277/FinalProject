namespace RevenueRecognitionSystem.Core.DTOs.ContractDTOs;

public record CreateContractRequest(
    int CustomerId,
    int SoftwareId,
    string? SoftwareVersion,
    DateTime StartDate,
    DateTime EndDate,
    int AdditionalSupportYears);

public record ContractPaymentRequest(
    int CustomerId,
    decimal Amount);
