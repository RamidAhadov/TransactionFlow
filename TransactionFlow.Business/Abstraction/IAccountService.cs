using TransactionFlow.Core.Utilities.Results;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.Business.Abstraction;

public interface IAccountService
{
    Task<IResult> CreateAccountAsync(Customer customer);
    Task<IResult> DeleteAccountAsync(Customer customer);
}