using FluentResults;
using TransactionFlow.Business.Models;

namespace TransactionFlow.BillingSystem.Services.Abstraction;

public interface ITransactionService
{
    Result<List<TransactionModel>> GetTransactions(int count);
    Result<List<TransactionModel>> GetAccountTransactions(int accountId, int count);
    Result<List<TransactionModel>> GetSentAccountTransactions(int accountId,int count);
    Result<List<TransactionModel>> GetReceivedAccountTransactions(int accountId, int count);
    Result<List<TransactionModel>> GetCustomerTransactions(int customerId, int count);
    Result<List<TransactionModel>> GetSentTransactions(int customerId,int count);
    Result<List<TransactionModel>> GetReceivedTransactions(int customerId, int count);
}