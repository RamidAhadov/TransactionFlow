using AutoMapper;
using FluentResults;
using TransactionFlow.BillingSystem.Models.Dtos;
using TransactionFlow.BillingSystem.Services.Abstraction;
using TransactionFlow.Business.Abstraction;
using TransactionFlow.Business.Models;
using TransactionFlow.Business.Models.Archive;
using TransactionFlow.Core.Constants;

namespace TransactionFlow.BillingSystem.Services.Concrete;

public class AccountService:IAccountService
{
    private IAccountManager _accountManager;
    private ICustomerManager _customerManager;
    private IArchiveManager _archiveManager;
    private IMapper _mapper;

    public AccountService(IAccountManager accountManager, ICustomerManager customerManager, IMapper mapper, IArchiveManager archiveManager)
    {
        _accountManager = accountManager;
        _customerManager = customerManager;
        _archiveManager = archiveManager;
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

    public async Task<Result> DeleteCustomerAsync(int customerId)
    {
        var getCustomerResult = _customerManager.GetCustomerById(customerId);
        if (getCustomerResult.IsFailed)
        {
            return Result.Fail(getCustomerResult.Errors);
        }

        var getAccountsResult = await _accountManager.GetAccountsAsync(getCustomerResult.Value);
        if (getCustomerResult.IsFailed)
        {
            return Result.Fail(getCustomerResult.Errors);
        }

        var deactivateResult = await _accountManager.DeactivateAccountAsync(getAccountsResult.Value);
        if (deactivateResult.IsFailed)
        {
            return Result.Fail(deactivateResult.Errors);
        }

        var customerArchiveModel = _mapper.Map<CustomerArchiveModel>(getCustomerResult.Value);
        customerArchiveModel.Accounts = _mapper.Map<List<CustomerAccountArchiveModel>>(getAccountsResult.Value);
        var archiveResult = await _archiveManager.ArchiveCustomerAndAccountsAsync(customerArchiveModel);
        if (archiveResult.IsFailed)
        {
            return Result.Fail(archiveResult.Errors);
        }

        var customerDeleteResult = _customerManager.Delete(getCustomerResult.Value);
        if (customerDeleteResult.IsFailed)
        {
            return Result.Fail(customerDeleteResult.Errors);
        }

        var accountDeleteResult = _accountManager.DeleteAccount(getAccountsResult.Value);
        if (accountDeleteResult.IsFailed)
        {
            return Result.Fail(accountDeleteResult.Errors);
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

    public async Task<Result> DeleteAccountAsync(int accountId)
    {
        var account = await _accountManager.GetAccountAsync(accountId);
        if (account.IsFailed)
        {
            return Result.Fail(account.Errors);
        }

        if (account.Value.IsMain)
        {
            var changeResult = await _accountManager.ChangeMainAccountAsync(account.Value);
            if (changeResult.IsFailed)
            {
                return Result.Fail(changeResult.Errors);
            }
        }

        var transferResult = await _accountManager.TransferToMainAsync(accountId);
        if (transferResult.IsFailed)
        {
            return Result.Fail(transferResult.Errors);
        }

        var deleteResult = await _accountManager.DeleteAccountAsync(account.Value);
        if (deleteResult.IsFailed)
        {
            return Result.Fail(deleteResult.Errors);
        }

        return Result.Ok();
    }

    public async Task<Result> DeactivateAccountAsync(int accountId)
    {
        var account = await _accountManager.GetAccountAsync(accountId);
        if (account.IsFailed)
        {
            return Result.Fail(account.Errors);
        }

        if (!account.Value.IsActive)
        {
            return Result.Fail(ErrorMessages.AccountAlreadyDeactivated);
        }
        
        if (account.Value.IsMain)
        {
            var changeResult = await _accountManager.ChangeMainAccountAsync(account.Value);
            if (changeResult.IsFailed)
            {
                return Result.Fail(changeResult.Errors);
            }
        }

        var transferResult = await _accountManager.TransferToMainAsync(accountId);
        if (transferResult.IsFailed)
        {
            return Result.Fail(transferResult.Errors);
        }

        var deactivateResult = await _accountManager.DeactivateAccountAsync(transferResult.Value);
        if (deactivateResult.IsFailed)
        {
            return Result.Fail(deactivateResult.Errors);
        }

        return Result.Ok();
    }

    public async Task<Result> ActivateAccountAsync(int accountId)
    {
        var account = await _accountManager.GetAccountAsync(accountId);
        if (account.IsFailed)
        {
            return Result.Fail(account.Errors);
        }

        if (account.Value.IsActive)
        {
            return Result.Fail(ErrorMessages.AccountAlreadyActivated);
        }
        
        var activateResult = await _accountManager.ActivateAccountAsync(account.Value);
        if (activateResult.IsFailed)
        {
            return Result.Fail(activateResult.Errors);
        }

        return Result.Ok();
    }
}