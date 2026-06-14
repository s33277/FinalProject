using Microsoft.EntityFrameworkCore;
using RevenueRecognitionSystem.Data.Entities.Authentication;
using RevenueRecognitionSystem.Data.Entities.Customers;
using RevenueRecognitionSystem.Data.Entities.Discounts;
using RevenueRecognitionSystem.Data.Entities.Contracts;
using RevenueRecognitionSystem.Data.Entities.SoftwareProducts;
using RevenueRecognitionSystem.Data.Entities.Subscriptions;

namespace RevenueRecognitionSystem.Data.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Software> Software { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<ContractPayment> ContractPayments { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<SubscriptionPayment> SubscriptionPayments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
