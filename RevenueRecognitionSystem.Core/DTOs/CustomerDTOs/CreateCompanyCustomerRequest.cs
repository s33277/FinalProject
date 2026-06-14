namespace RevenueRecognitionSystem.Core.DTOs.CustomerDTOs;

public record CreateCompanyCustomerRequest(
    string CompanyName,
    string KrsNumber,
    string Address,
    string Email,
    string PhoneNumber);