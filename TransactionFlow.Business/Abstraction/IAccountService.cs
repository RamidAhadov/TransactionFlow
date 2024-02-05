using TransactionFlow.Core.Utilities.Results;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.Business.Abstraction;

public interface IAccountService
{
    Task<IResult> CreateAccountAsync(Customer customer);
    Task<IResult> DeleteAccountAsync(Customer customer);
    Task<IResult> TryPositiveAdjust(Transaction transaction,CustomerAccount receiver);
    Task<IResult> TryNegativeAdjust(Transaction transaction,CustomerAccount sender);
    Task<IDataResult<CustomerAccount>> CheckSender(int senderId, decimal amount, decimal fee);
    Task<IDataResult<CustomerAccount>> CheckReceiver(int receiverId);
}