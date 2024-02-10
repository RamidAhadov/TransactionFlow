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
        var account = new CustomerAccount
        {
            CustomerId = customerModel.Id,
            Balance = 100,
            IsActive = true,
            IsMain = !await HasAccount(customerModel)
        };

        try
        {
            await _customerAccountDal.AddAsync(account);
            return Result.Ok();
        }
        catch (Exception)
        {
            return Result.Fail(ErrorMessages.AccountNotCreated);
        }
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

    public async Task<Result> DeleteAccountAsync(int customerId,int accountId)
    {
        // try
        // {
        //     //TODO
        //     var customerModel = new CustomerModel();
        //     
        //     var account = await _customerAccountDal.DeleteAsync(ca=>ca.AccountId == accountId);
        //     
        //     var selectResult = await SelectMainAccount(customerModel, _mapper.Map<CustomerAccountModel>(account));
        //     
        //     if (selectResult.IsFailed)
        //     {
        //         return Result.Fail(selectResult.Errors);
        //     }
        //     
        //     return Result.Ok();
        // }
        // catch (Exception)
        // {
        //     return Result.Fail(ErrorMessages.OperationFailed);
        // }

        return Result.Fail("");
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
}