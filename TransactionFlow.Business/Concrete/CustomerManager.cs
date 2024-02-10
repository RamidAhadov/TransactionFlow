using AutoMapper;
using FluentResults;
using TransactionFlow.Business.Abstraction;
using TransactionFlow.Business.Models;
using TransactionFlow.Core.Constants;
using TransactionFlow.DataAccess.Abstraction;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.Business.Concrete;

public class CustomerManager:ICustomerManager
{
    private readonly ICustomerDal _customerDal;

    private IMapper _mapper;

    public CustomerManager(ICustomerDal customerDal, IMapper mapper)
    {
        _customerDal = customerDal;
        _mapper = mapper;
    }

    public Result<List<CustomerModel>> GetAllCustomers()
    {
        return Result.Ok(_mapper.Map<List<CustomerModel>>(_customerDal.GetList()));
    }

    public Result Create(CustomerModel customer)
    {
        if (customer == null)
        {
            return Result.Fail(ErrorMessages.NullObjectEntered);
        }
            
        try
        {
            _customerDal.Add(_mapper.Map<Customer>(customer));
            return Result.Ok();
        }
        catch (Exception)
        {
            return Result.Fail(ErrorMessages.OperationFailed);
        }
    }

    public Result Update(CustomerModel customer)
    {
        if (customer == null)
        {
            return Result.Fail(ErrorMessages.NullObjectEntered);
        }
            
        try
        {
            _customerDal.Update(_mapper.Map<Customer>(customer));
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
    public async Task<Result<CustomerModel>> AddAsync(CustomerModel customer)
    {
        if (customer == null)
        {
            return Result.Fail(ErrorMessages.NullObjectEntered);
        }
            
        var createdCustomer = await _customerDal.AddAsync(_mapper.Map<Customer>(customer));
        
        return Result.Ok(_mapper.Map<CustomerModel>(createdCustomer));
    }

    public async Task<Result<CustomerModel>> DeleteCustomerAsync(int customerId)
    {
        if (customerId <= 0)
        {
            return Result.Fail(ErrorMessages.IndexOutOfTheRange);
        }
            
        var customer = await _customerDal.GetAsync(c => c.Id == customerId);
        if (customer == null)
        {
            return Result.Fail(ErrorMessages.ObjectNotFound);
        }
        
        try
        {
            await _customerDal.DeleteAsync(customer);
            return Result.Ok(_mapper.Map<CustomerModel>(customer));
        }
        catch (Exception)
        {
            return Result.Fail(ErrorMessages.OperationFailed);
        }
    }

    public Result<CustomerModel> GetCustomerById(int id)
    {
        if (id <= 0)
        {
            return Result.Fail(ErrorMessages.IndexOutOfTheRange);
        }
            
        var customer = _customerDal.Get(c => c.Id == id);
        if (customer == null)
        {
            return Result.Fail(ErrorMessages.ObjectNotFound);
        }
        
        return Result.Ok(_mapper.Map<CustomerModel>(customer));
    }
}