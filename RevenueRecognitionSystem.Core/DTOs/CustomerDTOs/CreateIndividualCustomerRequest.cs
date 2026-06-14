namespace RevenueRecognitionSystem.Core.DTOs.CustomerDTOs;

public record CreateIndividualCustomerRequest(
    string FirstName,
    string LastName,
    string Pesel, 
    string Address,
    string Email,
    string PhoneNumber);