using TransactionFlow.Core.DataAccess.EntityFramework;
using TransactionFlow.DataAccess.Abstraction;
using TransactionFlow.DataAccess.Concrete.EntityFramework.Contexts;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.DataAccess.Concrete.EntityFramework;

public class EfTransactionDal:EfEntityRepositoryBase<Transaction,TransactionContext>,ITransactionDal
{
    public List<Transaction> GetTransactions(int count)
    {
        using (var context = new TransactionContext())
        {
            return context.Transactions.OrderByDescending(t => t.Id).Take(count).ToList();
        }
    }

    public List<Transaction> GetTransactions(Customer customer, int? count)
    {
        using var context = new TransactionContext();
        return count == null
            ? context.Transactions.Where(t => t.SenderId == customer.Id || t.ReceiverId == customer.Id).ToList()
            : context.Transactions.Where(t => t.SenderId == customer.Id || t.ReceiverId == customer.Id).OrderByDescending(t => t.Id).Take(count.Value)
                .ToList();
    }

    public List<Transaction> GetSentTransactions(Customer customer, int? count)
    {
        using var context = new TransactionContext();
        return count == null
            ? context.Transactions.Where(t => t.SenderId == customer.Id).ToList()
            : context.Transactions.Where(t => t.SenderId == customer.Id).OrderByDescending(t => t.Id).Take(count.Value)
                .ToList();
    }

    public List<Transaction> GetReceivedTransactions(Customer customer, int? count)
    {
        using var context = new TransactionContext();
        return count == null
            ? context.Transactions.Where(t => t.ReceiverId == customer.Id).ToList()
            : context.Transactions.Where(t => t.ReceiverId == customer.Id).OrderByDescending(t => t.Id).Take(count.Value)
                .ToList();
    }
}