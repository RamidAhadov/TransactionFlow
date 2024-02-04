using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using TransactionFlow.Business.Abstraction;
using TransactionFlow.Business.Constants;
using TransactionFlow.Core.Utilities.Results;
using TransactionFlow.DataAccess.Abstraction;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.Business.Concrete;

public class CustomerService:ICustomerService
{
    private ICustomerDal _customerDal;

    public CustomerService(ICustomerDal customerDal)
    {
        _customerDal = customerDal;
    }

    public IDataResult<List<Customer>> GetAllCustomers()
    {
        return new SuccessDataResult<List<Customer>>(_customerDal.GetList());
    }

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

    public IResult Delete(Customer customer)
    {
        if (customer == null)
            return new ErrorResult(ErrorMessages.NullObjectEntered);
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
    public async Task<IResult> AddAsync(Customer customer)
    {
        if (customer == null)
            return new ErrorResult(ErrorMessages.NullObjectEntered);
        await _customerDal.AddAsync(customer);
        return new SuccessResult(InfoMessages.ItemAdded);
    }
    

    public IDataResult<Customer> GetCustomerById(int id)
    {
        if (id <= 0)
            return new ErrorDataResult<Customer>(ErrorMessages.IndexOutOfTheRange);
        var customer = _customerDal.Get(c => c.Id == id);
        if (customer == null)
            return new ErrorDataResult<Customer>(ErrorMessages.ObjectNotFound);
        return new SuccessDataResult<Customer>(customer);
    }

    
    [Description(description:"Count describes last *count transaction(s)")]
    public IDataResult<List<Transaction>> GetTransactions(Customer customer,int count)
    {
        if (customer == null)
            return new ErrorDataResult<List<Transaction>>(ErrorMessages.NullObjectEntered);
        var list = _customerDal.GetTransactions(customer,null);
        if (list == null)
            return new ErrorDataResult<List<Transaction>>(ErrorMessages.ObjectNotFound);
        if (list.Count == 0)
            return new ErrorDataResult<List<Transaction>>(InfoMessages.ZeroTransactionFound);
        return new SuccessDataResult<List<Transaction>>(list);
    }
}