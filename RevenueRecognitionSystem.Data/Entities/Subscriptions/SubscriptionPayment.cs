namespace RevenueRecognitionSystem.Data.Entities.Subscriptions;

public class SubscriptionPayment
{
    public int Id { get; set; }
    
    public int SubscriptionId { get; set; }
    public Subscription Subscription { get; set; } = null!;
    
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public DateTime CoveredPeriodStart { get; set; }
    public DateTime CoveredPeriodEnd { get; set; }
}