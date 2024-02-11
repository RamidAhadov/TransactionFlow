using FluentResults;
using TransactionFlow.Business.Models;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.Business.Abstraction;

public interface IAccountManager
{
    Task<Result> CreateAccountAsync(CustomerModel customer);
    Task<Result<List<CustomerAccountModel>>> GetAccountsAsync(CustomerModel customerModel);
    Task<Result> DeleteAccountAsync(CustomerAccountModel accountModel);
    Task<Result<CustomerAccountModel>> GetAccountAsync(int accountId);
    Task<Result> ChangeMainAccountAsync(CustomerAccountModel customerAccountModel);
    Task<Result<CustomerAccountModel>> TransferToMainAsync(int accountId);
    Task<Result> DeactivateAccountAsync(CustomerAccountModel accountModel);
    Task<Result> ActivateAccountAsync(CustomerAccountModel accountModel);
}