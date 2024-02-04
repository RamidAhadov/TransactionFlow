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
        catch (Exception e)
        {
            return new ErrorResult(ErrorMessages.OperationFailed);
        }
    }
}