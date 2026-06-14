using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RevenueRecognitionSystem.Data.Entities.Customers;

namespace RevenueRecognitionSystem.Data.Configurations;

public class CompanyCustomerConfiguration : IEntityTypeConfiguration<CompanyCustomer>
{
    public void Configure(EntityTypeBuilder<CompanyCustomer> builder)
    {
        builder.Property(c => c.CompanyName).IsRequired().HasMaxLength(200);
        builder.Property(c => c.KrsNumber).IsRequired().HasMaxLength(10);
        builder.HasIndex(c=>c.KrsNumber).IsUnique();
        builder.Property(c => c.KrsNumber).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
    }
}
