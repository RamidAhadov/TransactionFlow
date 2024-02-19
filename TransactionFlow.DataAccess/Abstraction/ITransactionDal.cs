using TransactionFlow.Core.DataAccess;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.DataAccess.Abstraction;

public interface ITransactionDal:IEntityRepository<Transaction>
{
    List<Transaction> GetTransactions(int count);
    List<Transaction> GetSentTransactions(int id,int count);
    List<Transaction> GetReceivedTransactions(int id,int count);
}