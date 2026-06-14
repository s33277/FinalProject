using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RevenueRecognitionSystem.Data.Entities.Contracts;

namespace RevenueRecognitionSystem.Data.Configurations.ContractConfigurations;

public class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.Property(c => c.SoftwareVersion).IsRequired().HasMaxLength(50);
        builder.Property(c => c.OriginalPrice).HasColumnType("decimal(18,2)");
        builder.Property(c => c.FinalPrice).HasColumnType("decimal(18,2)");

        builder.HasOne(c => c.Customer)
            .WithMany()
            .HasForeignKey(c => c.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Software)
            .WithMany()
            .HasForeignKey(c => c.SoftwareId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
