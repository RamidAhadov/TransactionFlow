using FluentResults;
using TransactionFlow.Business.Models;

namespace TransactionFlow.BillingSystem.Services.Abstraction;

public interface ITransactionService
{
    Result<List<TransactionModel>> GetTransactions(int count);
}