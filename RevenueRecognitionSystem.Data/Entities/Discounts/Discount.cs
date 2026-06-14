namespace RevenueRecognitionSystem.Data.Entities.Discounts;

public class Discount
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string OfferType { get; set; } = null!;
    public decimal ValuePercentage { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
}
