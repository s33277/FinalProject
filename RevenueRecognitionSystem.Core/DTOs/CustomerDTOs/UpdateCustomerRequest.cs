namespace RevenueRecognitionSystem.Core.DTOs.CustomerDTOs;

public record UpdateCustomerRequest(
    string Address,
    string Email,
    string PhoneNumber,
    string? FirstName,
    string? LastName,
    string? CompanyName);