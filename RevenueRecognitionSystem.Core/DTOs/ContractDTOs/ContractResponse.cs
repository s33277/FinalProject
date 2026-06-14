namespace RevenueRecognitionSystem.Core.DTOs.ContractDTOs;

public record ContractResponse(
    int Id,
    int CustomerId,
    int SoftwareId,
    string SoftwareVersion,
    DateTime StartDate,
    DateTime EndDate,
    decimal OriginalPrice,
    decimal FinalPrice,
    decimal AmountPaid,
    bool IsSigned);
