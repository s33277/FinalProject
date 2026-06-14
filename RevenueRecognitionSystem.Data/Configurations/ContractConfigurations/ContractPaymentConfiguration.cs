using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RevenueRecognitionSystem.Data.Entities.Contracts;

namespace RevenueRecognitionSystem.Data.Configurations.ContractConfigurations;

public class ContractPaymentConfiguration : IEntityTypeConfiguration<ContractPayment>
{
    public void Configure(EntityTypeBuilder<ContractPayment> builder)
    {
        builder.Property(cp => cp.Amount).HasColumnType("decimal(18,2)");
    }
}