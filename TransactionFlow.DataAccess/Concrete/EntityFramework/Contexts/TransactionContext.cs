using Microsoft.EntityFrameworkCore;
using TransactionFlow.Entities.Concrete;
using TransactionFlow.Entities.Concrete.Archive;

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
        modelBuilder.Entity<CustomerArchive>().HasKey(c => c.CustomerId);
        modelBuilder.Entity<CustomerAccountArchive>().HasKey(c => c.AccountId);
        
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<CustomerAccount> CustomerAccounts { get; set; }
    
    //Archive tables
    public DbSet<CustomerArchive> CustomersArchive { get; set; }
    public DbSet<CustomerAccountArchive> CustomerAccountsArchive { get; set; }
}