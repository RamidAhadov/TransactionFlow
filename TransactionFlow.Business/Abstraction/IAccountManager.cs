using FluentResults;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.Business.Abstraction;

public interface IAccountManager
{
    Task<Result> CreateAccountAsync(Customer customer);
    Task<Result> DeleteAccountAsync(Customer customer);
    Task<Result<CustomerAccount>> CheckSender(int senderId, decimal amount, decimal fee);
    Task<Result<CustomerAccount>> CheckReceiver(int receiverId);
    
}