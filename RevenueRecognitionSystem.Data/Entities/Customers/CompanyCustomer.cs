namespace RevenueRecognitionSystem.Data.Entities;

public class CompanyCustomer : Customer
{
    public string CompanyName { get; set; } = null!;
    public string KrsNumber { get; set; } = null!;
}