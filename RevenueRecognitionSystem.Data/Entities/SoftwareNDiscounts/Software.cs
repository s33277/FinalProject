namespace RevenueRecognitionSystem.Data.Entities.SoftwareNDiscounts;

public class Software
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string CurrentVersion { get; set; } = null!;
    public string Category { get; set; } = null!;
    public decimal BasePriceOneYear { get; set; }
    public decimal BasePriceSubscription { get; set; }
}