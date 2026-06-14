using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RevenueRecognitionSystem.Data.Entities.Subscriptions;

namespace RevenueRecognitionSystem.Data.Configurations.SubscriptionConfigurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
        builder.Property(s => s.PricePerPeriod).HasColumnType("decimal(18,2)");
        builder.Property(s => s.Status).HasConversion<string>().HasMaxLength(20);

        builder.HasOne(s => s.Customer)
            .WithMany()
            .HasForeignKey(s => s.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Software)
            .WithMany()
            .HasForeignKey(s => s.SoftwareId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
