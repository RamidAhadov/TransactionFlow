using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TransactionFlow.Entities.Concrete;
using TransactionFlow.Entities.Concrete.Archive;

namespace TransactionFlow.DataAccess.Concrete.EntityFramework.Contexts;

public class TransactionContext:DbContext
{
    private IConfiguration Configuration { get; set; }
    
    private ConnectionStringDetails _connectionString;
    
    public TransactionContext()
    {
        Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Development.json")
            .Build();
        _connectionString = Configuration.GetSection("ConnectionStringDetails").Get<ConnectionStringDetails>();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            $"Server={_connectionString.Host};Database={_connectionString.DatabaseName};User Id={_connectionString.UserName};Password={_connectionString.Password};TrustServerCertificate=True");
        optionsBuilder.EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>().HasKey(c => c.Id);
        modelBuilder.Entity<Transaction>().HasKey(t => t.Id);
        modelBuilder.Entity<CustomerAccount>().HasKey(ca => ca.AccountId);
        modelBuilder.Entity<CustomerArchive>().HasKey(c => c.CustomerId);
        modelBuilder.Entity<CustomerAccountArchive>().HasKey(c => c.AccountId);
        
        //ORM
        modelBuilder.Entity<CustomerAccount>()
            .HasOne(a => a.Customer)
            .WithMany(c => c.CustomerAccounts)
            .HasForeignKey(a => a.CustomerId);
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.CustomerAccount)
            .WithMany(ca => ca.SentTransactions)
            .HasForeignKey(t => t.SenderAccountId);
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.CustomerAccount)
            .WithMany(ca => ca.ReceivedTransactions)
            .HasForeignKey(t => t.ReceiverAccountId);
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<CustomerAccount> CustomerAccounts { get; set; }
    
    //Archive tables
    public DbSet<CustomerArchive> CustomersArchive { get; set; }
    public DbSet<CustomerAccountArchive> CustomerAccountsArchive { get; set; }
}