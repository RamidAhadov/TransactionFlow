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
            var account = context.CustomerAccounts
                .Include(ca => ca.SentTransactions)
                .FirstOrDefault(ca => ca.AccountId == id);

            if (account != null)
            {
                var transactions = count == 0
                    ? account.SentTransactions.OrderByDescending(t => t.Id).ToList()
                    : account.SentTransactions.OrderByDescending(t => t.Id).Take(count).ToList();

                return transactions;
            }

            return new List<Transaction>();
        }
    }

    public List<Transaction> GetReceivedTransactions(int id, int count)
    {
        using (var context = new TransactionContext())
        {
            var account = context.CustomerAccounts
                .Include(ca => ca.ReceivedTransactions)
                .FirstOrDefault(ca => ca.AccountId == id);

            if (account != null)
            {
                var transactions = count == 0
                    ? account.ReceivedTransactions.OrderByDescending(t => t.Id).ToList()
                    : account.ReceivedTransactions.OrderByDescending(t => t.Id).Take(count).ToList();

                return transactions;
            }

            return new List<Transaction>();
        }
    }
}