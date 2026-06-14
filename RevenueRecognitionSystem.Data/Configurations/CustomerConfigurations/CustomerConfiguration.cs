using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RevenueRecognitionSystem.Data.Entities.Customers;

namespace RevenueRecognitionSystem.Data.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.Property(c => c.Address).IsRequired().HasMaxLength(300);
        builder.Property(c => c.Email).IsRequired().HasMaxLength(254);
        builder.Property(c => c.PhoneNumber).IsRequired().HasMaxLength(30);

        builder.HasDiscriminator<string>("CustomerType")
            .HasValue<IndividualCustomer>("Individual")
            .HasValue<CompanyCustomer>("Company");
    }
}
