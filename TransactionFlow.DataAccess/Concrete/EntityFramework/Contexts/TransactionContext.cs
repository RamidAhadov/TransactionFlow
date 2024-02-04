using Microsoft.EntityFrameworkCore;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.DataAccess.Concrete.EntityFramework.Contexts;

public class TransactionContext:DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            @"Server=localhost,1433;Database=TransactionFlow;User Id=SA;Password=Password1!;TrustServerCertificate=True");
        optionsBuilder.EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>().HasKey(c => c.Id);
        modelBuilder.Entity<Transaction>().HasKey(t => t.Id);
        modelBuilder.Entity<CustomerAccount>().HasKey(ca => ca.AccountId);
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<CustomerAccount> CustomerAccounts { get; set; }
}