using FluentResults;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.Business.Abstraction;

public interface ITransactionManager
{
    Result<List<Transaction>> GetTransactions(Customer customer, int? count);
    Result<List<Transaction>> GetSentTransactions(Customer customer, int? count);
    Result<List<Transaction>> GetReceivedTransactions(Customer customer, int? count);
    Task<Result<Transaction>> CreateTransaction(int senderId, int receiverId, decimal amount, decimal serviceFee);
    Task<Result> ChangeTransactionStatus(Transaction transaction);

}