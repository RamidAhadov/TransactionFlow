using System.Diagnostics;
using AutoMapper;
using FluentResults;
using Microsoft.Extensions.Configuration;
using NLog;
using NuGet.Protocol;
using TransactionFlow.Business.Abstraction;
using TransactionFlow.Business.Models;
using TransactionFlow.Core.Constants;
using TransactionFlow.DataAccess.Abstraction;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.Business.Concrete;

public class CustomerManager:ICustomerManager
{
    private readonly CustomerDetails? _details;
    private readonly ICustomerDal _customerDal;
    private readonly IMapper _mapper;
    private readonly Logger _logger;

    public CustomerManager(ICustomerDal customerDal, IMapper mapper, IConfiguration configuration)
    {
        _customerDal = customerDal;
        _mapper = mapper;
        _details = configuration.GetSection("CustomerDetails").Get<CustomerDetails>();
        _logger = LogManager.GetLogger("CustomerManagerLogger");
    }

    public Result<List<CustomerModel>> GetAllCustomers()
    {
        return Result.Ok(_mapper.Map<List<CustomerModel>>(_customerDal.GetList()));
    }

    public Result Create(CustomerModel? customer)
    {
        var sw = Stopwatch.StartNew();
        if (customer == null)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(Create), Message = $"{nameof(customer)} is null" }.ToJson());
            
            return Result.Fail(ErrorMessages.NullObjectEntered);
        }
        
        try
        {
            _customerDal.Add(_mapper.Map<Customer>(customer));
            _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(Create), Message = "Customer created.",CustomerID = customer.Id }.ToJson());
            
            return Result.Ok();
        }
        catch (Exception exception)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(Create), Message = exception.InnerException?.Message ?? exception.Message, CustomerID = customer.Id }.ToJson());
            
            return Result.Fail(ErrorMessages.OperationFailed);
        }
    }

    public Result Update(CustomerModel? customer)
    {
        var sw = Stopwatch.StartNew();
        if (customer == null)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(Update), Message = $"{nameof(customer)} is null" }.ToJson());
            
            return Result.Fail(ErrorMessages.NullObjectEntered);
        }
            
        try
        {
            _customerDal.Update(_mapper.Map<Customer>(customer));
            _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(Update), Message = "Customer updated.",CustomerID = customer.Id }.ToJson());
            
            return Result.Ok();
        }
        catch (Exception exception)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(Update), Message = exception.InnerException?.Message ?? exception.Message,CustomerID = customer.Id }.ToJson());
            
            return Result.Fail(ErrorMessages.OperationFailed);
        }
    }
    
    public Result Delete(int id)
    {
        var sw = Stopwatch.StartNew();
        var customer = _customerDal.Get(c => c.Id == id);
        if (customer == null)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(Delete), Message = $"Customer not exists with id {id}" }.ToJson());
            
            return Result.Fail(ErrorMessages.AccountNotFound);
        }
            
        try
        {
            _customerDal.Delete(customer);
            _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(Delete), Message = "Customer deleted.",CustomerID = id }.ToJson());
            
            return Result.Ok();
        }
        catch (Exception exception)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(Delete), Message = exception.InnerException?.Message ?? exception.Message,CustomerID = id }.ToJson());
            
            return Result.Fail(ErrorMessages.OperationFailed);
        }
    }

    public Result Delete(CustomerModel customerModel)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            _customerDal.Delete(_mapper.Map<Customer>(customerModel));
            
            _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(Delete), Message = "Customer deleted." ,CustomerID = customerModel.Id}.ToJson());
            
            return Result.Ok();
        }
        catch (Exception exception)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(Delete), Message = exception.InnerException?.Message ?? exception.Message ,CustomerID = customerModel.Id}.ToJson());
            
            return Result.Fail(ErrorMessages.OperationFailed);
        }
    }

    //To add a customer asynchronously.
    public async Task<Result<CustomerModel>> CreateAsync(CustomerModel? customer)
    {
        var sw = Stopwatch.StartNew();
        if (customer == null)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(CreateAsync), Message = $"{nameof(customer)} is null." }.ToJson());
            
            return Result.Fail(ErrorMessages.NullObjectEntered);
        }

        try
        {
            var createdCustomer = await _customerDal.AddAsync(_mapper.Map<Customer>(customer));
            _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(CreateAsync), Message = "Customer created." ,CustomerID = customer.Id}.ToJson());
            
            return Result.Ok(_mapper.Map<CustomerModel>(createdCustomer));
        }
        catch (Exception exception)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(CreateAsync), Message = exception.InnerException?.Message ?? exception.Message ,CustomerID = customer.Id}.ToJson());
            
            return Result.Fail(ErrorMessages.OperationFailed);
        }
    }

    public Result<CustomerModel> GetCustomerById(int id)
    {
        var sw = Stopwatch.StartNew();
        if (id <= 0)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(GetCustomerById), Message = "Given ID is less than zero.",Id = id }.ToJson());
            
            return Result.Fail(ErrorMessages.IndexOutOfTheRange);
        }
            
        var customer = _customerDal.Get(c => c.Id == id);
        
        if (customer == null)
        {
            _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(GetCustomerById), Message = $"Customer with ID: {id} not found." }.ToJson());
            
            return Result.Fail(ErrorMessages.ObjectNotFound);
        }

        var customerModel = _mapper.Map<CustomerModel>(customer);
        customerModel.MaxAllowedAccounts = _details.MaxAllowedAccounts;
        _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(GetCustomerById), Message = "Customer retrieved.", Customer = customerModel.ToJson() }.ToJson());
        
        return Result.Ok(customerModel);
    }
    public Result<CustomerModel> GetCustomerWithAccounts(int customerId)
    {
        var sw = Stopwatch.StartNew();
        if (customerId <= 0)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(GetCustomerWithAccounts), Message = "Given ID is less than zero.", Id = customerId }.ToJson());
            
            return Result.Fail(ErrorMessages.IndexOutOfTheRange);
        }
        
        try
        {
            var customer = _customerDal.GetCustomerWithAccounts(customerId);
            var model = _mapper.Map<CustomerModel>(customer);
            model.Accounts = _mapper.Map<List<CustomerAccountModel>>(customer.CustomerAccounts);
            model.MaxAllowedAccounts = _details.MaxAllowedAccounts;
            _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(GetCustomerWithAccounts), Message = "Customer retrieved.", Customer = model.ToJson() }.ToJson());
            
            return Result.Ok(model);
        }
        catch (Exception exception)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(GetCustomerWithAccounts), Message = exception.InnerException?.Message ?? exception.Message, CustomerId = customerId }.ToJson());
            
            return Result.Fail(ErrorMessages.AccountsNotFound);
        }
    }
}