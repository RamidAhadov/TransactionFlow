using TransactionFlow.Core.DataAccess;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.DataAccess.Abstraction;

public interface ICustomerDal:IEntityRepository<Customer>
{
    List<Transaction> GetTransactions(Customer customer, int? count);
    List<Transaction> GetSentTransactions(Customer customer, int? count);
    List<Transaction> GetReceivedTransactions(Customer customer, int? count);
}