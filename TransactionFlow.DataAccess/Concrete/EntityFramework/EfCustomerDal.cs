using Microsoft.EntityFrameworkCore;
using TransactionFlow.Core.DataAccess.EntityFramework;
using TransactionFlow.DataAccess.Abstraction;
using TransactionFlow.DataAccess.Concrete.EntityFramework.Contexts;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.DataAccess.Concrete.EntityFramework;

public class EfCustomerDal:EfEntityRepositoryBase<Customer,TransactionContext>,ICustomerDal
{
    public List<CustomerAccount> GetAccounts(Customer customer)
    {
        using (var context = new TransactionContext())
        {
            return context.CustomerAccounts.Where(ca => ca.CustomerId == customer.Id)
                .OrderByDescending(ca => ca.AccountId).ToList();
        }
    }

    public async Task<List<CustomerAccount>> GetAccountsAsync(Customer customer)
    {
        await using (var context = new TransactionContext())
        {
            var customers = await context.CustomerAccounts.Where(ca => ca.CustomerId == customer.Id)
                .OrderByDescending(ca => ca.AccountId).ToListAsync();
            return customers;
        }
    }
}