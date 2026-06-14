using RevenueRecognitionSystem.Data.Entities.SoftwareNDiscounts;

namespace RevenueRecognitionSystem.Data.Entities.Subscriptions;

public class Subscription
{
    public int Id { get; set; }
    
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    
    public int SoftwareId { get; set; }
    public Software Software { get; set; } = null!;
    
    public string Name { get; set; } = null!;
    public decimal PricePerPeriod { get; set; }
    public int RenewalPeriodMonths { get; set; }
    public DateTime ValidUntil { get; set; }
    public bool IsCancelled { get; set; }
    
    public ICollection<SubscriptionPayment> Payments { get; set; } = new List<SubscriptionPayment>();
}