using FluentResults;
using TransactionFlow.Business.Abstraction;
using TransactionFlow.Core.Constants;
using TransactionFlow.DataAccess.Abstraction;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.Business.Concrete;

public class CustomerAccountManager:IAccountManager
{
    private ICustomerAccountDal _customerAccountDal;

    public CustomerAccountManager(ICustomerAccountDal customerAccountDal)
    {
        _customerAccountDal = customerAccountDal;
    }

    public async Task<Result> CreateAccountAsync(Customer customer)
    {
        var account = new CustomerAccount
        {
            CustomerId = customer.Id,
            Balance = 100,
            IsActive = true
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

    public async Task<Result> DeleteAccountAsync(Customer customer)
    {
        try
        {
            var account = await _customerAccountDal.GetAsync(ca => ca.CustomerId == customer.Id);
            await _customerAccountDal.DeleteAsync(account);
            return Result.Ok();
        }
        catch (Exception)
        {
            return Result.Fail(ErrorMessages.OperationFailed);
        }
    }

    public async Task<Result<CustomerAccount>> CheckSender(int senderId, decimal amount, decimal fee)
    {
        if (amount <= 0 || fee < 0 || senderId < 0)
        {
            return Result.Fail(ErrorMessages.IncorrectFormat);
        }
        
        var account = await _customerAccountDal.GetAsync(ca => ca.CustomerId == senderId);
        if (account == null)
        {
            return Result.Fail<CustomerAccount>(ErrorMessages.AccountNotFound);
        }

        if (account.Balance < amount + fee)
        {
            return Result.Fail<CustomerAccount>(InfoMessages.InsufficientFund);
        }
            
        return Result.Ok(account);
    }

    public async Task<Result<CustomerAccount>> CheckReceiver(int receiverId)
    {
        if (receiverId < 0)
        {
            return Result.Fail(ErrorMessages.IncorrectFormat);
        }
            
        var account = await _customerAccountDal.GetAsync(ca => ca.CustomerId == receiverId);
        if (account == null)
        {
            return Result.Fail(ErrorMessages.AccountNotFound);
        }

        if (!account.IsActive)
        {
            return Result.Fail(InfoMessages.InactiveAccount);
        }
            
        return Result.Ok(account);
    }
}