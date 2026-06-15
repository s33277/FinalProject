namespace RevenueRecognitionSystem.Data.Entities.Customers;

public class Customer
{
    public int Id { get; set; }
    public string Address { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public bool IsDeleted { get; set; }
}
