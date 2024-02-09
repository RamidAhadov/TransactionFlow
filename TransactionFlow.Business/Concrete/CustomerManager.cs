using FluentResults;
using TransactionFlow.Business.Abstraction;
using TransactionFlow.Core.Constants;
using TransactionFlow.DataAccess.Abstraction;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.Business.Concrete;

public class CustomerManager:ICustomerManager
{
    private readonly ICustomerDal _customerDal;
    //private static readonly ILog _log = LogManager.GetLogger(typeof(CustomerManager));

    public CustomerManager(ICustomerDal customerDal)
    {
        _customerDal = customerDal;
    }

    public Result<List<Customer>> GetAllCustomers()
    {
        return Result.Ok(_customerDal.GetList());
    }

    //[LogAspect(typeof(FileLogger))]
    public Result Add(Customer customer)
    {
        if (customer == null)
        {
            return Result.Fail(ErrorMessages.NullObjectEntered);
        }
            
        try
        {
            _customerDal.Add(customer);
            return Result.Ok();
        }
        catch (Exception)
        {
            return Result.Fail(ErrorMessages.OperationFailed);
        }
    }

    public Result Update(Customer customer)
    {
        if (customer == null)
        {
            return Result.Fail(ErrorMessages.NullObjectEntered);
        }
            
        try
        {
            _customerDal.Update(customer);
            return Result.Ok();
        }
        catch (Exception)
        {
            return Result.Fail(ErrorMessages.OperationFailed);
        }
    }
    
    public Result Delete(int id)
    {
        var customer = _customerDal.Get(c => c.Id == id);
        if (customer == null)
        {
            return Result.Fail(ErrorMessages.AccountNotFound);
        }
            
        try
        {
            _customerDal.Delete(customer);
            return Result.Ok();
        }
        catch (Exception)
        {
            return Result.Fail(ErrorMessages.OperationFailed);
        }
    }

    //To add a customer asynchronously.
    public async Task<Result<Customer>> AddAsync(Customer customer)
    {
        if (customer == null)
        {
            return Result.Fail(ErrorMessages.NullObjectEntered);
        }
            
        var createdCustomer = await _customerDal.AddAsync(customer);
        
        return Result.Ok(createdCustomer);
    }

    public async Task<Result<Customer>> DeleteCustomerAsync(int customerId)
    {
        if (customerId <= 0)
        {
            return Result.Fail(ErrorMessages.IndexOutOfTheRange);
        }
            
        var customer = await _customerDal.GetAsync(c => c.Id == customerId);
        if (customer == null)
            return Result.Fail(ErrorMessages.ObjectNotFound);
        
        try
        {
            await _customerDal.DeleteAsync(customer);
            return Result.Ok(customer);
        }
        catch (Exception)
        {
            return Result.Fail(ErrorMessages.OperationFailed);
        }
    }

    //[LogAspect(typeof(FileLogger))]
    public Result<Customer> GetCustomerById(int id)
    {
        //_log.Info("This is a log message.");   
        if (id <= 0)
        {
            return Result.Fail(ErrorMessages.IndexOutOfTheRange);
        }
            
        var customer = _customerDal.Get(c => c.Id == id);
        if (customer == null)
            return Result.Fail(ErrorMessages.ObjectNotFound);
        
        return Result.Ok(customer);
    }
}