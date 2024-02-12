using TransactionFlow.Core.DataAccess.EntityFramework;
using TransactionFlow.DataAccess.Abstraction;
using TransactionFlow.DataAccess.Concrete.EntityFramework.Contexts;
using TransactionFlow.Entities.Concrete.Archive;

namespace TransactionFlow.DataAccess.Concrete.EntityFramework;

public class EfArchiveDal:EfEntityRepositoryBase<CustomerArchive,TransactionContext>,IArchiveDal
{
    public async Task ArchiveAsync(CustomerArchive customer,List<CustomerAccountArchive> customerAccount)
    {
        await using (var context = new TransactionContext())
        {
            await using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    var addCustomer = context.CustomersArchive.AddAsync(customer).AsTask();
                    var addAccounts = context.AddRangeAsync(customerAccount);

                    await Task.WhenAll(addCustomer, addAccounts);

                    await context.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
    }
}