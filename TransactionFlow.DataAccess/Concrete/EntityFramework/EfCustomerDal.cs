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
        return await GetAccountsByIdAsync(customer.Id);
    }

    public async Task<List<CustomerAccount>> GetAccountsAsync(int customerId)
    {
        return await GetAccountsByIdAsync(customerId);
    }
    
    
    private static async Task<List<CustomerAccount>> GetAccountsByIdAsync(int customerId)
    {
        await using (var context = new TransactionContext())
        {
            //Temporary try-catch
            //Test here
            try
            {
                var customers = await context.CustomerAccounts.Where(ca => ca.CustomerId == customerId)
                    .OrderByDescending(ca => ca.AccountId).ToListAsync();
                return customers;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}