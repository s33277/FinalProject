using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RevenueRecognitionSystem.Data.Entities.Customers;

namespace RevenueRecognitionSystem.Data.Configurations;

public class IndividualCustomerConfiguration : IEntityTypeConfiguration<IndividualCustomer>
{
    public void Configure(EntityTypeBuilder<IndividualCustomer> builder)
    {
        builder.Property(c => c.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(c => c.LastName).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Pesel).IsRequired().HasMaxLength(11);
        builder.HasIndex(c=>c.Pesel).IsUnique();
        builder.Property(c => c.Pesel).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
    }
}
