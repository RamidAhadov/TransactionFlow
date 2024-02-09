using FluentResults;
using TransactionFlow.BillingSystem.Services.Abstraction;
using TransactionFlow.Business.Abstraction;

namespace TransactionFlow.BillingSystem.Services.Concrete;

public class TransferService:ITransferService
{
    private IAccountManager _accountManager;
    private ITransactionManager _transactionManager;

    public TransferService(IAccountManager accountManager, ITransactionManager transactionManager)
    {
        _accountManager = accountManager;
        _transactionManager = transactionManager;
    }

    public async Task<Result> TransferMoneyAsync(int senderId, int receiverId, decimal amount, decimal fee)
    {
        return Result.Ok();
    }
}