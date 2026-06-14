using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RevenueRecognitionSystem.Data.Entities.Subscriptions;

namespace RevenueRecognitionSystem.Data.Configurations.SubscriptionConfigurations;

public class SubscriptionPaymentConfiguration : IEntityTypeConfiguration<SubscriptionPayment>
{
    public void Configure(EntityTypeBuilder<SubscriptionPayment> builder)
    {
        builder.Property(sp => sp.Amount).HasColumnType("decimal(18,2)");
    }
}