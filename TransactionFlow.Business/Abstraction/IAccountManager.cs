using FluentResults;
using TransactionFlow.Business.Models;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.Business.Abstraction;

public interface IAccountManager
{
    Task<Result> CreateAccountAsync(CustomerModel customer);
    Task<Result<List<CustomerAccountModel>>> GetAccountsAsync(CustomerModel customerModel);
    Result<CustomerAccountModel> GetMainAccount(int customerId);
    Task<Result> DeleteAccountAsync(CustomerAccountModel accountModel);
    Result DeleteAccount(List<CustomerAccountModel> accountModels);
    Task<Result<CustomerAccountModel>> GetAccountAsync(int accountId);
    Task<Result> ChangeMainAccountAsync(CustomerAccountModel customerAccountModel);
    Task<Result<CustomerAccountModel>> TransferToMainAsync(TransactionModel transactionModel);
    Task<Result> DeactivateAccountAsync(CustomerAccountModel accountModel);
    Task<Result> DeactivateAccountAsync(List<CustomerAccountModel> accountModels);
    Task<Result> ActivateAccountAsync(CustomerAccountModel accountModel);
}