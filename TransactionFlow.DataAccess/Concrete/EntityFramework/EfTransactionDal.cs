using Microsoft.EntityFrameworkCore;
using TransactionFlow.Core.DataAccess.EntityFramework;
using TransactionFlow.DataAccess.Abstraction;
using TransactionFlow.DataAccess.Concrete.EntityFramework.Contexts;
using TransactionFlow.Entities.Concrete;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.

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

    public List<Transaction> GetSentTransactions(int id, int count)
    {
        using (var context = new TransactionContext())
        {
            return count == 0
                ? context.CustomerAccounts.Include(ca => ca.SentTransactions).FirstOrDefault(ca => ca.AccountId == id)
                    .SentTransactions.OrderByDescending(t => t.Id).ToList()
                : context.CustomerAccounts.Include(ca => ca.SentTransactions).FirstOrDefault(ca => ca.AccountId == id)
                    .SentTransactions.OrderByDescending(t => t.Id).Take(count).ToList();
        }
    }

    public List<Transaction> GetReceivedTransactions(int id, int count)
    {
        using (var context = new TransactionContext())
        {
            return count == 0
                ? context.CustomerAccounts.Include(ca => ca.ReceivedTransactions).FirstOrDefault(ca => ca.AccountId == id)
                    .ReceivedTransactions.OrderByDescending(t => t.Id).ToList()
                : context.CustomerAccounts.Include(ca => ca.ReceivedTransactions).FirstOrDefault(ca => ca.AccountId == id)
                    .ReceivedTransactions.OrderByDescending(t => t.Id).Take(count).ToList();
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