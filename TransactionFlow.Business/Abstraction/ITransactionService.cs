using TransactionFlow.Core.DataAccess;
using TransactionFlow.Core.Utilities.Results;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.Business.Abstraction;

public interface ITransactionService
{
    IDataResult<List<Transaction>> GetTransactions(Customer customer, int? count);
    IDataResult<List<Transaction>> GetSentTransactions(Customer customer, int? count);
    IDataResult<List<Transaction>> GetReceivedTransactions(Customer customer, int? count);
    Task<IDataResult<Transaction>> CreateTransaction(int senderId, int receiverId, decimal amount, decimal serviceFee);
    Task<IResult> ChangeTransactionStatus(Transaction transaction);

}