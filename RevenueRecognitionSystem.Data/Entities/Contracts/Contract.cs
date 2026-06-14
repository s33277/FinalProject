using RevenueRecognitionSystem.Data.Entities.Customers;
using RevenueRecognitionSystem.Data.Entities.SoftwareProducts;

namespace RevenueRecognitionSystem.Data.Entities.Contracts;

public class Contract
{
    public int Id { get; set; }

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public int SoftwareId { get; set; }
    public Software Software { get; set; } = null!;

    public string SoftwareVersion { get; set; } = null!;

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public int AdditionalSupportYears { get; set; }
    public decimal OriginalPrice { get; set; }
    public decimal FinalPrice { get; set; }
    public bool IsSigned { get; set; }

    public ICollection<ContractPayment> Payments { get; set; } = new List<ContractPayment>();
}
