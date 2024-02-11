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

    public async Task<Result> CreateAccountAsync(CustomerModel customerModel)
    {
        var accountModel = new CustomerAccountModel
        {
            CustomerId = customerModel.Id,
            Balance = 100,
            IsActive = true,
            IsMain = !await HasAccount(customerModel)
        };

        try
        {
            await _customerAccountDal.AddAsync(_mapper.Map<CustomerAccount>(accountModel));
            return Result.Ok();
        }
        catch (Exception)
        {
            return Result.Fail(ErrorMessages.AccountNotCreated);
        }
    }

    public void Do(string message)
    {
        
    }

    public async Task<Result<List<CustomerAccountModel>>> GetAccountsAsync(CustomerModel customerModel)
    {
        try
        {
            var accounts = await GetAccountListAsync(customerModel);
            return Result.Ok(accounts);
        }
        catch (Exception)
        {
            return Result.Fail(ErrorMessages.OperationFailed);
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
            return Result.Fail(ErrorMessages.OperationFailed);
        }
    }

    public async Task<Result<CustomerAccountModel>> GetAccountAsync(int accountId)
    {
        if (accountId==null)
        {
            return Result.Fail(ErrorMessages.NullObjectEntered);
        }
        
        var account = _mapper.Map<CustomerAccountModel>(await _customerAccountDal.GetAsync(ca => ca.AccountId == accountId));
        
        if (account== null)
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

    public async Task<Result<CustomerAccountModel>> TransferToMainAsync(int accountId)
    {
        try
        {
            var account = await _customerAccountDal.TransferToMainAsync(accountId);

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

    private async Task<Result> SelectMainAccount(CustomerModel customerModel, CustomerAccountModel accountModel)
    {
        if (accountModel.IsMain && await HasAccount(customerModel))
        {
            var secondAccount = (await GetAccountListAsync(customerModel))[0];
            secondAccount.IsMain = true;

            try
            {
                await _customerAccountDal.UpdateAsync(_mapper.Map<CustomerAccount>(secondAccount));
                return Result.Ok();
            }
            catch (Exception e)
            {
                return Result.Fail(e.Message);
            }
        }

        return Result.Ok();
    }
    
    private async Task<bool> HasAccount(CustomerModel customerModel)
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
        return _mapper.Map<List<CustomerAccountModel>>(
            await _customerDal.GetAccountsAsync(customerId));
    }
}