namespace RevenueRecognitionSystem.Data.Entities.Customers;

public class CompanyCustomer : Customer
{
    public string CompanyName { get; set; } = null!;
    public string KrsNumber { get; set; } = null!;
}