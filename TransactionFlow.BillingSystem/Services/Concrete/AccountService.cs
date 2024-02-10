using AutoMapper;
using FluentResults;
using TransactionFlow.BillingSystem.Models.Dtos;
using TransactionFlow.BillingSystem.Services.Abstraction;
using TransactionFlow.Business.Abstraction;
using TransactionFlow.Business.Models;
using TransactionFlow.Core.Constants;

namespace TransactionFlow.BillingSystem.Services.Concrete;

public class AccountService:IAccountService
{
    private IAccountManager _accountManager;
    private ICustomerManager _customerManager;
    private IMapper _mapper;

    public AccountService(IAccountManager accountManager, ICustomerManager customerManager, IMapper mapper)
    {
        _accountManager = accountManager;
        _customerManager = customerManager;
        _mapper = mapper;
    }

    public async Task<Result> CreateCustomerAsync(CustomerDto customerDto)
    {
        var customerCreateResult = await _customerManager.CreateAsync(_mapper.Map<CustomerModel>(customerDto));
        if (customerCreateResult.IsFailed)
        {
            return Result.Fail(customerCreateResult.Errors);
        }

        var accountCreateResult = await _accountManager.CreateAccountAsync(customerCreateResult.Value);
        if (accountCreateResult.IsFailed)
        {
            return Result.Fail(accountCreateResult.Errors);
        }

        return Result.Ok();
    }

    public async Task<Result> CreateAccountAsync(int customerId)
    {
        var customer = _customerManager.GetCustomerById(customerId);
        if (customer.IsFailed)
        {
            return Result.Fail(customer.Errors);
        }

        var accounts = await _accountManager.GetAccountsAsync(customer.Value);
        if (accounts.Value.Count >= customer.Value.MaxAllowedAccounts)
        {
            return Result.Fail(ErrorMessages.MaxAllowedAccountsExceed);
        }

        var createdAccountResult = await _accountManager.CreateAccountAsync(customer.Value);
        if (createdAccountResult.IsFailed)
        {
            return Result.Fail(createdAccountResult.Errors);
        }
        
        return Result.Ok();
    }
}