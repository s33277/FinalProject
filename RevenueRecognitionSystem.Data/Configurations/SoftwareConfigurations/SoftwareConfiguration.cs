using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RevenueRecognitionSystem.Data.Entities.SoftwareProducts;

namespace RevenueRecognitionSystem.Data.Configurations.SoftwareConfigurations;

public class SoftwareConfiguration : IEntityTypeConfiguration<Software>
{
    public void Configure(EntityTypeBuilder<Software> builder)
    {
        builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
        builder.Property(s => s.Description).HasMaxLength(1000);
        builder.Property(s => s.CurrentVersion).IsRequired().HasMaxLength(50);
        builder.Property(s => s.Category).IsRequired().HasMaxLength(100);
        builder.Property(s => s.BasePriceOneYear).HasColumnType("decimal(18,2)");
        builder.Property(s=>s.BasePriceSubscription).HasColumnType("decimal(18,2)");
    }
}
