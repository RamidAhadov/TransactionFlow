using FluentResults;
using TransactionFlow.Business.Models;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.Business.Abstraction;

public interface IAccountManager
{
    Task<Result> CreateAccountAsync(CustomerModel customer);
    Task<Result<List<CustomerAccountModel>>> GetAccountsAsync(CustomerModel customerModel);
    Task<Result> DeleteAccountAsync(int accountId);
    Task<Result<CustomerAccountModel>> GetAccountAsync(int accountId);
    Task<Result> ChangeMainAccountAsync(CustomerAccountModel customerAccountModel);
    Task<Result> TransferToMainAsync(int accountId);
}