using AutoMapper;
using FluentResults;
using TransactionFlow.Business.Abstraction;
using TransactionFlow.Business.Models;
using TransactionFlow.Core.Constants;
using TransactionFlow.DataAccess.Abstraction;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.Business.Concrete;

public class CustomerAccountManager:IAccountManager
{
    private ICustomerAccountDal _customerAccountDal;
    private ICustomerDal _customerDal;
    private IMapper _mapper;

    public CustomerAccountManager(ICustomerAccountDal customerAccountDal, ICustomerDal customerDal, IMapper mapper)
    {
        _customerAccountDal = customerAccountDal;
        _customerDal = customerDal;
        _mapper = mapper;
    }

    public Result CreateAccount(CustomerModel customerModel)
    {
        var accountModel = new CustomerAccountModel
        {
            CustomerId = customerModel.Id,
            Balance = 100,
            IsActive = true,
            IsMain = !HasAccount(customerModel),
            CreatedDate = DateTime.Now
        };

        try
        {
            var newAccount = _mapper.Map<CustomerAccount>(accountModel);
            _customerAccountDal.Add(newAccount);
            return Result.Ok();
        }
        catch (Exception)
        {
            return Result.Fail(ErrorMessages.AccountNotCreated);
        }
    }

    public async Task<Result> CreateAccountAsync(CustomerModel customerModel)
    {
        var accountModel = new CustomerAccountModel
        {
            CustomerId = customerModel.Id,
            Balance = 100,
            IsActive = true,
            IsMain = !await HasAccountAsync(customerModel),
            CreatedDate = DateTime.Now
        };

        try
        {
            var newAccount = _mapper.Map<CustomerAccount>(accountModel);
            await _customerAccountDal.AddAsync(newAccount);
            return Result.Ok();
        }
        catch (Exception)
        {
            return Result.Fail(ErrorMessages.AccountNotCreated);
        }
    }

    public async Task<Result<List<CustomerAccountModel>>> GetAccountsAsync(int customerId)
    {
        try
        {
            var accounts = await GetAccountListAsync(customerId);
            return Result.Ok(accounts);
        }
        catch (Exception)
        {
            return Result.Fail(ErrorMessages.AccountsNotFound);
        }
    }

    
    public Result<CustomerAccountModel> GetMainAccount(int customerId)
    {
        try
        {
            return Result.Ok(_mapper.Map<CustomerAccountModel>(_customerDal.GetCustomerWithAccounts(customerId).CustomerAccounts.FirstOrDefault(ca => ca.IsMain)));
        }
        catch (Exception)
        {
            return Result.Fail(ErrorMessages.AccountNotFound);
        }
    }

    public async Task<Result<List<CustomerAccountModel>>> GetAccountsByAccountAsync(int accountId)
    {
        var customerId = _customerAccountDal.Get(ca => ca.AccountId == accountId).CustomerId;
        try
        {
            var accounts = await GetAccountListAsync(customerId);
            if (accounts == null || accounts.Count == 0)
            {
                return Result.Fail(ErrorMessages.AccountNotFound);
            }
            
            return Result.Ok(accounts);
        }
        catch (Exception)
        {
            return Result.Fail(ErrorMessages.AccountsNotFound);
        }
    }

    public async Task<Result> DeleteAccountAsync(CustomerAccountModel accountModel)
    {
        try
        {
            await _customerAccountDal.DeleteAsync(_mapper.Map<CustomerAccount>(accountModel));
            
            return Result.Ok();
        }
        catch (Exception)
        {
            return Result.Fail(ErrorMessages.AccountNotDeleted);
        }
    }

    public Result DeleteAccount(List<CustomerAccountModel> accountModels)
    {
        try
        {
            _customerAccountDal.DeleteRange(_mapper.Map<List<CustomerAccount>>(accountModels));
            
            return Result.Ok();
        }
        catch (Exception)
        {
            return Result.Fail(ErrorMessages.AccountNotDeleted);
        }
    }

    public Result<CustomerAccountModel> GetAccount(int accountId)
    {
        if (accountId==null)
        {
            return Result.Fail(ErrorMessages.NullObjectEntered);
        }
        
        var account = _mapper.Map<CustomerAccountModel>( _customerAccountDal.Get(ca => ca.AccountId == accountId));

        if (account == null)
        {
            return Result.Fail(ErrorMessages.AccountNotFound);
        }

        return Result.Ok(account);
    }

    public async Task<Result> ChangeMainAccountAsync(CustomerAccountModel customerAccountModel)
    {
        try
        {
            await _customerAccountDal.ChangeMainAccountAsync(customerAccountModel.CustomerId);

            return Result.Ok();
        }
        catch (Exception)
        {
            return Result.Fail(ErrorMessages.OperationFailed);
        }
    }

    public async Task<Result<CustomerAccountModel>> TransferToMainAsync(TransactionModel transactionModel)
    {
        try
        {
            var account = await _customerAccountDal.TransferAsync(_mapper.Map<TransactionDetails>(transactionModel));

            return Result.Ok(_mapper.Map<CustomerAccountModel>(account));
        }
        catch (Exception e)
        {
            return Result.Fail(e.Message);
        }
    }

    public async Task<Result> DeactivateAccountAsync(CustomerAccountModel accountModel)
    {
        try
        {
            accountModel.IsActive = false;
            await _customerAccountDal.UpdateAsync(_mapper.Map<CustomerAccount>(accountModel));
            
            return Result.Ok();
        }
        catch (Exception)
        {
            return Result.Fail(ErrorMessages.OperationFailed);
        }
    }

    public async Task<Result> DeactivateAccountAsync(List<CustomerAccountModel> accountModels)
    {
        try
        {
            await _customerAccountDal.UpdateRangeAsync(_mapper.Map<List<CustomerAccount>>(accountModels));
            
            return Result.Ok();
        }
        catch (Exception)
        {
            return Result.Fail(ErrorMessages.OperationFailed);
        }
    }

    public async Task<Result> ActivateAccountAsync(CustomerAccountModel accountModel)
    {
        try
        {
            accountModel.IsActive = true;
            await _customerAccountDal.UpdateAsync(_mapper.Map<CustomerAccount>(accountModel));
            
            return Result.Ok();
        }
        catch (Exception)
        {
            return Result.Fail(ErrorMessages.OperationFailed);
        }
    }

    public async Task<Result> TransferMoneyAsync(TransactionModel transactionModel)
    {
        try
        {
            await _customerAccountDal.TransferAsync(_mapper.Map<TransactionDetails>(transactionModel));

            return Result.Ok();
        }
        catch (Exception e)
        {
            return Result.Fail(e.Message);
        }
    }

    private bool HasAccount(CustomerModel customerModel)
    {
        var accountList = GetAccountList(customerModel);

        return accountList.Count != 0;
    }
    
    private async Task<bool> HasAccountAsync(CustomerModel customerModel)
    {
        var accountList = await GetAccountListAsync(customerModel);

        return accountList.Count != 0;
    }

    private async Task<List<CustomerAccountModel>> GetAccountListAsync(CustomerModel customerModel)
    {
        return _mapper.Map<List<CustomerAccountModel>>(
            await _customerDal.GetAccountsAsync(_mapper.Map<Customer>(customerModel)));
    }
    
    private async Task<List<CustomerAccountModel>> GetAccountListAsync(int customerId)
    {
        return _mapper.Map<List<CustomerAccountModel>>(await _customerDal.GetAccountsAsync(customerId));
    }

    private List<CustomerAccountModel> GetAccountList(CustomerModel customerModel)
    {
        return _mapper.Map<List<CustomerAccountModel>>(_customerDal.GetAccounts(_mapper.Map<Customer>(customerModel)));
    }
}