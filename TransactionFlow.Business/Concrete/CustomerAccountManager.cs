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
            IsActive = true
        };

        account.IsMain = !CheckHasMain(customerModel);
        
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

    public async Task<Result> DeleteAccountAsync(CustomerModel customerModel,int accountId)
    {
        try
        {
            var account =
                await _customerAccountDal.GetAsync(ca =>
                    ca.CustomerId == customerModel.Id && ca.AccountId == accountId);
            await _customerAccountDal.DeleteAsync(account);
            
            
            //Create another method.
            //var accountModel = _mapper.Map<CustomerAccountModel>(account);
            // if (accountModel.IsMain && CheckHasMain(customerModel))
            // {
            //     var secondAccount = GetAccountList(customerModel)[0];
            //     secondAccount.IsMain = true;
            // }
            return Result.Ok();
        }
        catch (Exception)
        {
            return Result.Fail(ErrorMessages.OperationFailed);
        }
    }

    private bool CheckHasMain(CustomerModel customerModel)
    {
        var accountList = GetAccountList(customerModel);

        return accountList.Count != 0;
    }

    private List<CustomerAccountModel> GetAccountList(CustomerModel customerModel)
    {
        return _mapper.Map<List<CustomerAccountModel>>(_customerDal.GetAccounts(_mapper.Map<Customer>(customerModel)));
    }
    
    
}