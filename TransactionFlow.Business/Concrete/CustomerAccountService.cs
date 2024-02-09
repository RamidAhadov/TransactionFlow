using TransactionFlow.Business.Abstraction;
using TransactionFlow.Business.Constants;
using TransactionFlow.Core.Utilities.Results;
using TransactionFlow.DataAccess.Abstraction;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.Business.Concrete;

public class CustomerAccountService:IAccountService
{
    private ICustomerAccountDal _customerAccountDal;

    public CustomerAccountService(ICustomerAccountDal customerAccountDal)
    {
        _customerAccountDal = customerAccountDal;
    }

    public async Task<IResult> CreateAccountAsync(Customer customer)
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
            return new SuccessResult();
        }
        catch (Exception)
        {
            return new ErrorResult(ErrorMessages.AccountNotCreated);
        }
    }

    public async Task<IResult> DeleteAccountAsync(Customer customer)
    {
        try
        {
            var account = await _customerAccountDal.GetAsync(ca => ca.CustomerId == customer.Id);
            await _customerAccountDal.DeleteAsync(account);
            return new SuccessResult();
        }
        catch (Exception)
        {
            return new ErrorResult(ErrorMessages.OperationFailed);
        }
    }

    public async Task<IResult> TryPositiveAdjust(Transaction transaction,CustomerAccount receiver)
    {
        if (receiver == null)
        {
            return new ErrorResult(ErrorMessages.NullObjectEntered);
        }
        receiver.Balance += transaction.TransactionAmount;
        try
        {
            await _customerAccountDal.UpdateAsync(receiver);
            return new SuccessResult(SuccessMessages.BalanceAdjusted);
        }
        catch (Exception e)
        {
            return new ErrorResult(ErrorMessages.AdjustmentFailed);
        }
    }

    public async Task<IResult> TryNegativeAdjust(Transaction transaction,CustomerAccount sender)
    {
        if (transaction == null)
        {
            return new ErrorResult(ErrorMessages.NullObjectEntered);
        }
        sender.Balance = transaction.ServiceFee == 0
            ? sender.Balance - transaction.TransactionAmount
            : sender.Balance - transaction.TransactionAmount - transaction.ServiceFee;
        try
        {
            await _customerAccountDal.UpdateAsync(sender);
            return new SuccessResult(SuccessMessages.BalanceAdjusted);
        }
        catch (Exception e)
        {
            return new ErrorResult(ErrorMessages.AdjustmentFailed);
        }
    }

    public async Task<IDataResult<CustomerAccount>> CheckSender(int senderId, decimal amount, decimal fee)
    {
        if (amount <= 0 || fee < 0 || senderId < 0)
        {
            return new ErrorDataResult<CustomerAccount>(ErrorMessages.IncorrectFormat);
        }
        
        var account = await _customerAccountDal.GetAsync(ca => ca.CustomerId == senderId);
        if (account == null)
        {
            return new ErrorDataResult<CustomerAccount>(ErrorMessages.AccountNotFound);
        }

        if (account.Balance < amount + fee)
        {
            return new ErrorDataResult<CustomerAccount>(InfoMessages.InsufficientFund);
        }
            
        return new SuccessDataResult<CustomerAccount>(account);
    }

    public async Task<IDataResult<CustomerAccount>> CheckReceiver(int receiverId)
    {
        if (receiverId < 0)
        {
            return new ErrorDataResult<CustomerAccount>(ErrorMessages.IncorrectFormat);
        }
            
        var account = await _customerAccountDal.GetAsync(ca => ca.CustomerId == receiverId);
        if (account == null)
        {
            return new ErrorDataResult<CustomerAccount>(ErrorMessages.AccountNotFound);
        }

        if (!account.IsActive)
        {
            return new ErrorDataResult<CustomerAccount>(InfoMessages.InactiveAccount);
        }
            
        return new SuccessDataResult<CustomerAccount>(account);
    }
}