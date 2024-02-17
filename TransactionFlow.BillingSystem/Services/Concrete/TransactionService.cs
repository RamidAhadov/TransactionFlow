using FluentResults;
using TransactionFlow.BillingSystem.Services.Abstraction;
using TransactionFlow.Business.Abstraction;
using TransactionFlow.Business.Models;

namespace TransactionFlow.BillingSystem.Services.Concrete;

public class TransactionService:ITransactionService
{
    private ITransactionManager _transactionManager;

    public TransactionService(ITransactionManager transactionManager)
    {
        _transactionManager = transactionManager;
    }

    public Result<List<TransactionModel>> GetTransactions(int count)
    {
        var transactionResult = _transactionManager.GetTransactions(count);
        if (transactionResult.IsFailed)
        {
            return Result.Fail(transactionResult.Errors);
        }
        
        return Result.Ok(transactionResult.Value);
    }
}