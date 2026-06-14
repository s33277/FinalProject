namespace RevenueRecognitionSystem.Data.Entities.Contracts;

public class ContractPayment
{
    public int Id { get; set; }
    
    public int ContractId { get; set; }
    public Contract Contract { get; set; } = null!;
    
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    
    public bool IsRefunded { get; set; }
}