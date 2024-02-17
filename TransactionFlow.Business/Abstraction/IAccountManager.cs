using FluentResults;
using TransactionFlow.Business.Models;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.Business.Abstraction;

public interface IAccountManager
{
    Task<Result> CreateAccountAsync(CustomerModel customer);
    Task<Result<List<CustomerAccountModel>>> GetAccountsAsync(int customerId);
    Result<CustomerAccountModel> GetMainAccount(int customerId);
    Task<Result<List<CustomerAccountModel>>> GetAccountsByAccountAsync(int accountId);
    Task<Result<CustomerAccountModel>> GetMainAccountAsync(int customerId);
    Task<Result> DeleteAccountAsync(CustomerAccountModel accountModel);
    Result DeleteAccount(List<CustomerAccountModel> accountModels);
    Result<CustomerAccountModel> GetAccount(int accountId);
    Task<Result> ChangeMainAccountAsync(CustomerAccountModel customerAccountModel);
    Task<Result<CustomerAccountModel>> TransferToMainAsync(TransactionModel transactionModel);
    Task<Result> DeactivateAccountAsync(CustomerAccountModel accountModel);
    Task<Result> DeactivateAccountAsync(List<CustomerAccountModel> accountModels);
    Task<Result> ActivateAccountAsync(CustomerAccountModel accountModel);
    Task<Result> TransferMoneyAsync(TransactionModel transactionModel);
}