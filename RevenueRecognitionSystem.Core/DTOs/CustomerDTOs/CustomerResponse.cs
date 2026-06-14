namespace RevenueRecognitionSystem.Core.DTOs.CustomerDTOs;

public record CustomerResponse(
    int Id,
    string CustomerType,
    string Name,
    string Address,
    string Email,
    string PhoneNumber);