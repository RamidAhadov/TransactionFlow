using System.ComponentModel;
using log4net;
using TransactionFlow.Business.Abstraction;
using TransactionFlow.Business.Constants;
using TransactionFlow.Core.Aspects.Postsharp.LogAspects;
using TransactionFlow.Core.CrossCuttingConcerns.Logging.Log4Net.Loggers;
using TransactionFlow.Core.Utilities.Results;
using TransactionFlow.DataAccess.Abstraction;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.Business.Concrete;

public class CustomerService:ICustomerService
{
    private readonly ICustomerDal _customerDal;
    //private static readonly ILog _log = LogManager.GetLogger(typeof(CustomerService));

    public CustomerService(ICustomerDal customerDal)
    {
        _customerDal = customerDal;
    }

    public IDataResult<List<Customer>> GetAllCustomers()
    {
        return new SuccessDataResult<List<Customer>>(_customerDal.GetList());
    }

    //[LogAspect(typeof(FileLogger))]
    public IResult Add(Customer customer)
    {
        if (customer == null)
            return new ErrorResult(ErrorMessages.NullObjectEntered);
        try
        {
            _customerDal.Add(customer);
            return new SuccessResult(InfoMessages.ItemAdded);
        }
        catch (Exception)
        {
            return new ErrorResult(ErrorMessages.OperationFailed);
        }
    }

    public IResult Update(Customer customer)
    {
        if (customer == null)
            return new ErrorResult(ErrorMessages.NullObjectEntered);
        try
        {
            _customerDal.Update(customer);
            return new SuccessResult(InfoMessages.ItemUpdated);
        }
        catch (Exception)
        {
            return new ErrorResult(ErrorMessages.OperationFailed);
        }
    }
    
    public IResult Delete(int id)
    {
        var customer = _customerDal.Get(c => c.Id == id);
        if (customer == null)
            return new ErrorResult(ErrorMessages.ObjectNotFound);
        try
        {
            _customerDal.Delete(customer);
            return new SuccessResult(InfoMessages.ItemDeleted);
        }
        catch (Exception)
        {
            return new ErrorResult(ErrorMessages.OperationFailed);
        }
    }

    //To add a customer asynchronously.
    public async Task<IDataResult<Customer>> AddAsync(Customer customer)
    {
        if (customer == null)
            return new ErrorDataResult<Customer>(ErrorMessages.NullObjectEntered);
        var createdCustomer = await _customerDal.AddAsync(customer);
        return new SuccessDataResult<Customer>(createdCustomer,InfoMessages.ItemAdded);
    }

    public async Task<IDataResult<Customer>> DeleteCustomerAsync(Customer customer)
    {
        if (customer == null)
            return new ErrorDataResult<Customer>(ErrorMessages.NullObjectEntered);
        try
        {
            await _customerDal.DeleteAsync(customer);
            return new SuccessDataResult<Customer>(customer);
        }
        catch (Exception)
        {
            return new ErrorDataResult<Customer>(ErrorMessages.OperationFailed);
        }
    }

    public async Task<IDataResult<Customer>> DeleteCustomerAsync(int customerId)
    {
        if (customerId <= 0)
            return new ErrorDataResult<Customer>(ErrorMessages.IndexOutOfTheRange);
        var customer = await _customerDal.GetAsync(c => c.Id == customerId);
        if (customer == null)
            return new ErrorDataResult<Customer>(ErrorMessages.ObjectNotFound);
        try
        {
            await _customerDal.DeleteAsync(customer);
            return new SuccessDataResult<Customer>(customer);
        }
        catch (Exception)
        {
            return new ErrorDataResult<Customer>(ErrorMessages.OperationFailed);
        }
    }

    //[LogAspect(typeof(FileLogger))]
    public IDataResult<Customer> GetCustomerById(int id)
    {
        //_log.Info("This is a log message.");   
        if (id <= 0)
            return new ErrorDataResult<Customer>(ErrorMessages.IndexOutOfTheRange);
        var customer = _customerDal.Get(c => c.Id == id);
        if (customer == null)
            return new ErrorDataResult<Customer>(ErrorMessages.ObjectNotFound);
        return new SuccessDataResult<Customer>(customer);
    }
}