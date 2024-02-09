using FluentResults;

namespace TransactionFlow.BillingSystem.Services.Abstraction;

public interface ITransferService
{
    Task<Result> TransferMoneyAsync(int senderId, int receiverId, decimal amount, decimal fee);
}