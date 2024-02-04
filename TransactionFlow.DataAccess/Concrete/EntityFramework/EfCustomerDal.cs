using TransactionFlow.Core.DataAccess.EntityFramework;
using TransactionFlow.DataAccess.Abstraction;
using TransactionFlow.DataAccess.Concrete.EntityFramework.Contexts;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.DataAccess.Concrete.EntityFramework;

public class EfCustomerDal:EfEntityRepositoryBase<Customer,TransactionContext>,ICustomerDal
{
    public List<Transaction> GetTransactions(Customer customer, int? count)
    {
        using var context = new TransactionContext();
        return count == null
            ? context.Transactions.Where(t => t.CustomerId == customer.Id).ToList()
            : context.Transactions.Where(t => t.CustomerId == customer.Id).OrderByDescending(t => t.Id).Take(count.Value)
                .ToList();
    }
}