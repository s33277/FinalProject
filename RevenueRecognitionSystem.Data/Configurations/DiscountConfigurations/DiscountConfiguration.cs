using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RevenueRecognitionSystem.Data.Entities.Discounts;

namespace RevenueRecognitionSystem.Data.Configurations.DiscountConfigurations;

public class DiscountConfiguration : IEntityTypeConfiguration<Discount>
{
    public void Configure(EntityTypeBuilder<Discount> builder)
    {
        builder.Property(d => d.Name).IsRequired().HasMaxLength(200);
        builder.Property(d => d.OfferType).IsRequired().HasMaxLength(50);
        builder.Property(d => d.ValuePercentage).HasColumnType("decimal(5,2)");
    }
}
