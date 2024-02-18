using FluentResults;
using TransactionFlow.Business.Models;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.Business.Abstraction;

public interface ITransactionManager
{
    Result<List<TransactionModel>> GetTransactions(int count);
    Result<List<TransactionModel>> GetSentAccountTransactions(int accountId, int count);
    Result<List<TransactionModel>> GetReceivedAccountTransactions(int accountId, int count);
    Result<List<TransactionModel>> GetSentTransactions(List<CustomerAccountModel> accounts, int count);
    Result<List<TransactionModel>> GetReceivedTransactions(List<CustomerAccountModel> accounts, int count);
    Result<List<Transaction>> GetTransactions(Customer customer, int? count);
    Result<List<Transaction>> GetSentTransactions(Customer customer, int? count);
    Result<List<Transaction>> GetReceivedTransactions(Customer customer, int? count);

    Task<Result<TransactionModel>> CreateTransactionAsync(TransferParticipants participants, decimal amount,
        decimal serviceFee, short transactionType);
    Task<Result> ChangeTransactionStatus(Transaction transaction);

}