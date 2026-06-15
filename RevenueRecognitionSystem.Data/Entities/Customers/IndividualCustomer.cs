namespace RevenueRecognitionSystem.Data.Entities.Customers;

public class IndividualCustomer : Customer
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Pesel { get; set; } = null!;
}
