using AutoMapper;
using FluentResults;
using NLog;
using NuGet.Protocol;
using TransactionFlow.BillingSystem.Models.Dtos;
using TransactionFlow.BillingSystem.Services.Abstraction;
using TransactionFlow.Business.Abstraction;
using TransactionFlow.Business.Models;
using TransactionFlow.Business.Models.Archive;
using TransactionFlow.Core.Constants;

namespace TransactionFlow.BillingSystem.Services.Concrete;

public class AccountService:IAccountService
{
    private readonly IAccountManager _accountManager;
    private readonly ICustomerManager _customerManager;
    private readonly IArchiveManager _archiveManager;
    private readonly ITransactionManager _transactionManager;
    private readonly IMapper _mapper;
    private readonly Logger _logger;

    public AccountService(
        IAccountManager accountManager,
        ICustomerManager customerManager,
        IMapper mapper,
        IArchiveManager archiveManager,
        ITransactionManager transactionManager)
    {
        _accountManager = accountManager;
        _customerManager = customerManager;
        _archiveManager = archiveManager;
        _transactionManager = transactionManager;
        _mapper = mapper;
        _logger = LogManager.GetLogger("AccountServiceLogger");
    }

    public Result<List<CustomerModel>> GetAllCustomers()
    {
        var customersResult = _customerManager.GetAllCustomers();
        if (customersResult.IsFailed)
        {
            _logger.Error(new {customersResult.Errors, Method = nameof(GetAllCustomers)}.ToJson());
            
            return Result.Fail(customersResult.Errors);
        }
        _logger.Info(new {Method = nameof(GetAllCustomers),customersResult.Value}.ToJson());
        
        return Result.Ok(customersResult.Value);
    }

    public async Task<Result> CreateCustomerAsync(CustomerDto customerDto)
    {
        var customerCreateResult = await _customerManager.CreateAsync(_mapper.Map<CustomerModel>(customerDto));
        if (customerCreateResult.IsFailed)
        {
            _logger.Error(new {customerCreateResult.Errors, Method = nameof(CreateCustomerAsync)}.ToJson());

            return Result.Fail(customerCreateResult.Errors);
        }

        var accountCreateResult = await _accountManager.CreateAccountAsync(customerCreateResult.Value);
        if (accountCreateResult.IsFailed)
        {
            _logger.Error(new {Message = accountCreateResult.Errors, Method = nameof(CreateCustomerAsync)}.ToJson());
            
            return Result.Fail(accountCreateResult.Errors);
        }
        _logger.Info(new {Message = "Customer successfully created",Method = nameof(CreateCustomerAsync), CreatedCustomer = customerDto.ToJson()}.ToJson());
        
        return Result.Ok();
    }
    
    public Result UpdateCustomer(int customerId, CustomerDto customerDto)
    {
        var customerModel = _mapper.Map<CustomerModel>(customerDto);
        customerModel.Id = customerId;
        var updateResult = _customerManager.Update(customerModel);
        if (updateResult.IsFailed)
        {
            _logger.Error(new {Message = updateResult.Errors,Method = nameof(UpdateCustomer), Customer = customerDto.ToJson()}.ToJson());
            
            return Result.Fail(updateResult.Errors);
        }
        _logger.Info(new {Message = "Customer updated.",Method = nameof(UpdateCustomer), Customer = customerModel.ToJson()}.ToJson());
        
        return Result.Ok();
    }

    public async Task<Result> DeleteCustomerAsync(int customerId)
    {
        var getCustomerResult = _customerManager.GetCustomerWithAccounts(customerId);
        var accountIds = getCustomerResult.Value.Accounts?.Select(ca => ca.AccountId).ToArray();
        if (getCustomerResult.IsFailed)
        {
            _logger.Error(new {getCustomerResult.Errors, Method = nameof(DeleteCustomerAsync), CustomerId = customerId}.ToJson());
            
            return Result.Fail(getCustomerResult.Errors);
        }

        var deactivateResult = await _accountManager.DeactivateAccountAsync(getCustomerResult.Value.Accounts);
        if (deactivateResult.IsFailed)
        {
            _logger.Error(new {deactivateResult.Errors, Method = nameof(DeleteCustomerAsync), CustomerId = customerId}.ToJson());
            
            return Result.Fail(deactivateResult.Errors);
        }

        var customerArchiveModel = _mapper.Map<CustomerArchiveModel>(getCustomerResult.Value);
        customerArchiveModel.Accounts = _mapper.Map<List<CustomerAccountArchiveModel>>(getCustomerResult.Value.Accounts);
        var archiveResult = await _archiveManager.ArchiveCustomerAndAccountsAsync(customerArchiveModel);
        if (archiveResult.IsFailed)
        {
            _logger.Error(new {archiveResult.Errors, Method = nameof(DeleteCustomerAsync), CustomerId = customerId}.ToJson());
            
            return Result.Fail(archiveResult.Errors);
        }

        var customerDeleteResult = _customerManager.Delete(getCustomerResult.Value);
        if (customerDeleteResult.IsFailed)
        {
            _logger.Error(new {customerDeleteResult.Errors, Method = nameof(DeleteCustomerAsync), CustomerId = customerId}.ToJson());
            
            return Result.Fail(customerDeleteResult.Errors);
        }

        var accountDeleteResult = _accountManager.DeleteAccount(getCustomerResult.Value.Accounts);
        if (accountDeleteResult.IsFailed)
        {
            _logger.Error(new {accountDeleteResult.Errors, Method = nameof(DeleteCustomerAsync), CustomerId = customerId}.ToJson());
            
            return Result.Fail(accountDeleteResult.Errors);
        }
        _logger.Info(new {Message = "Customer and accounts deactivated and archived.", Method = nameof(DeleteCustomerAsync), CustomerId = customerId, Accounts = accountIds.ToJson()}.ToJson());
        
        return Result.Ok();
    }

    public Result CreateAccount(int customerId)
    {
        var customerResult = _customerManager.GetCustomerWithAccounts(customerId);
        if (customerResult.IsFailed)
        {
            _logger.Error(new {Message = customerResult.Errors, Method = nameof(CreateAccount),CustomerId = customerId}.ToJson());
            
            return Result.Fail(customerResult.Errors);
        }

        if (customerResult.Value.Accounts?.Count >= customerResult.Value.MaxAllowedAccounts)
        {
            _logger.Error(new {Message = customerResult.Errors, Method = nameof(CreateAccount),CustomerId = customerId}.ToJson());
            
            return Result.Fail(ErrorMessages.MaxAllowedAccountsExceed);
        }

        var createdAccountResult = _accountManager.CreateAccount(customerResult.Value);
        if (createdAccountResult.IsFailed)
        {
            _logger.Error(new {Message = customerResult.Errors, Method = nameof(CreateAccount),CustomerId = customerId}.ToJson());
            
            return Result.Fail(createdAccountResult.Errors);
        }
        _logger.Info(new {Message = "Account created.", CustomerId = customerId, createdAccountResult.Value.AccountId}.ToJson());
        
        return Result.Ok();
    }

    public async Task<Result> DeleteAccountAsync(int accountId)
    {
        var accountResult = _accountManager.GetAccount(accountId);
        if (accountResult.IsFailed)
        {
            _logger.Error(new{Message = accountResult.Errors,Method = nameof(DeleteAccountAsync), AccountId = accountId}.ToJson());
            
            return Result.Fail(accountResult.Errors);
        }

        if (accountResult.Value.IsMain)
        {
            var changeResult = await _accountManager.ChangeMainAccountAsync(accountResult.Value);
            if (changeResult.IsFailed)
            {
                _logger.Error(new{Message = changeResult.Errors,Method = nameof(DeleteAccountAsync), AccountId = accountId}.ToJson());
                
                return Result.Fail(changeResult.Errors);
            }
        }

        var mainAccountResult = _accountManager.GetMainAccount(accountResult.Value.CustomerId);
        if (mainAccountResult.IsFailed)
        {
            _logger.Error(new{Message = mainAccountResult.Errors,Method = nameof(DeleteAccountAsync), AccountId = accountId}.ToJson());
            
            return Result.Fail(mainAccountResult.Errors);
        }

        var transferParticipants = new TransferParticipants
        {
            SenderId = accountResult.Value.CustomerId,
            SenderAccountId = accountId,
            ReceiverId = accountResult.Value.CustomerId,
            ReceiverAccountId = mainAccountResult.Value.AccountId
        };
        var transactionResult = await _transactionManager.CreateTransactionAsync(transferParticipants,accountResult.Value.Balance,default,1);
        if (transactionResult.IsFailed)
        {
            _logger.Error(new{Message = transactionResult.Errors,Method = nameof(DeleteAccountAsync), AccountId = accountId}.ToJson());
            
            return Result.Fail(transactionResult.Errors);
        }
        
        var transferResult = await _accountManager.TransferToMainAsync(transactionResult.Value);
        if (transferResult.IsFailed)
        {
            _logger.Error(new{Message = transferResult.Errors,Method = nameof(DeleteAccountAsync), AccountId = accountId}.ToJson());
            
            return Result.Fail(transferResult.Errors);
        }

        var archiveResult =
            await _archiveManager.ArchiveAccountAsync(_mapper.Map<CustomerAccountArchiveModel>(accountResult.Value));
        if (archiveResult.IsFailed)
        {
            _logger.Error(new{Message = archiveResult.Errors,Method = nameof(DeleteAccountAsync), AccountId = accountId}.ToJson());
            
            return Result.Fail(archiveResult.Errors);
        }

        var deleteResult = await _accountManager.DeleteAccountAsync(accountResult.Value);
        if (deleteResult.IsFailed)
        {
            _logger.Error(new{Message = deleteResult.Errors,Method = nameof(DeleteAccountAsync), AccountId = accountId}.ToJson());
            
            return Result.Fail(deleteResult.Errors);
        }
        _logger.Info(new {Message = "Account has been deleted and archived.",Method = nameof(DeleteAccountAsync), AccountId = accountId}.ToJson());

        return Result.Ok();
    }

    public async Task<Result> DeactivateAccountAsync(int accountId)
    {
        var accountResult = _accountManager.GetAccount(accountId);
        if (accountResult.IsFailed)
        {
            _logger.Error(new {Message = accountResult.Errors, Method = nameof(DeactivateAccountAsync), AccountId = accountId}.ToJson());
            
            return Result.Fail(accountResult.Errors);
        }

        if (!accountResult.Value.IsActive)
        {
            _logger.Warn(new {Message = ErrorMessages.AccountAlreadyDeactivated, Method = nameof(DeactivateAccountAsync), AccountId = accountId}.ToJson());

            return Result.Fail(ErrorMessages.AccountAlreadyDeactivated);
        }
        
        if (accountResult.Value.IsMain)
        {
            var changeResult = await _accountManager.ChangeMainAccountAsync(accountResult.Value);
            if (changeResult.IsFailed)
            {
                _logger.Error(new {Message = changeResult.Errors, Method = nameof(DeactivateAccountAsync), AccountId = accountId}.ToJson());

                return Result.Fail(changeResult.Errors);
            }
        }

        var mainAccountResult = _accountManager.GetMainAccount(accountResult.Value.CustomerId);
        if (mainAccountResult.IsFailed)
        {
            _logger.Error(new {Message = mainAccountResult.Errors, Method = nameof(DeactivateAccountAsync), AccountId = accountId}.ToJson());

            return Result.Fail(mainAccountResult.Errors);
        }
        
        var transferParticipants = new TransferParticipants
        {
            SenderId = accountResult.Value.CustomerId,
            SenderAccountId = accountId,
            ReceiverId = accountResult.Value.CustomerId,
            ReceiverAccountId = mainAccountResult.Value.AccountId
        };
        var transactionResult = await _transactionManager.CreateTransactionAsync(transferParticipants,accountResult.Value.Balance,default,1);
        if (transactionResult.IsFailed)
        {
            _logger.Error(new {Message = transactionResult.Errors, Method = nameof(DeactivateAccountAsync), AccountId = accountId}.ToJson());

            return Result.Fail(transactionResult.Errors);
        }

        var transferResult = await _accountManager.TransferToMainAsync(transactionResult.Value);
        if (transferResult.IsFailed)
        {
            _logger.Error(new {Message = transferResult.Errors, Method = nameof(DeactivateAccountAsync), AccountId = accountId}.ToJson());

            return Result.Fail(transferResult.Errors);
        }

        var deactivateResult = await _accountManager.DeactivateAccountAsync(transferResult.Value);
        if (deactivateResult.IsFailed)
        {
            _logger.Error(new {Message = deactivateResult.Errors, Method = nameof(DeactivateAccountAsync), AccountId = accountId}.ToJson());

            return Result.Fail(deactivateResult.Errors);
        }
        _logger.Info(new {Message = "Account deactivated.", Method = nameof(DeactivateAccountAsync), AccountId = accountId}.ToJson());

        return Result.Ok();
    }

    public async Task<Result> ActivateAccountAsync(int accountId)
    {
        var accountResult = _accountManager.GetAccount(accountId);
        if (accountResult.IsFailed)
        {
            _logger.Error(new {Message = accountResult.Errors, Method = nameof(ActivateAccountAsync), AccountId = accountId}.ToJson());

            return Result.Fail(accountResult.Errors);
        }

        if (accountResult.Value.IsActive)
        {
            _logger.Error(new {Message = ErrorMessages.AccountAlreadyActivated, Method = nameof(ActivateAccountAsync), AccountId = accountId}.ToJson());
            
            return Result.Fail(ErrorMessages.AccountAlreadyActivated);
        }
        
        var activateResult = await _accountManager.ActivateAccountAsync(accountResult.Value);
        if (activateResult.IsFailed)
        {
            _logger.Error(new {Message = activateResult.Errors, Method = nameof(ActivateAccountAsync), AccountId = accountId}.ToJson());
            
            return Result.Fail(activateResult.Errors);
        }
        _logger.Info(new {Message = "Account activated.", Method = nameof(ActivateAccountAsync), AccountId = accountId}.ToJson());

        return Result.Ok();
    }

    public Result<CustomerModel> GetCustomer(int customerId)
    {
        var customerResult = _customerManager.GetCustomerWithAccounts(customerId);
        if (customerResult.IsFailed)
        {
            _logger.Error(new {Message = customerResult.Errors, Method = nameof(GetCustomer), CustomerId = customerId}.ToJson());
            
            return Result.Fail(customerResult.Errors);
        }
        _logger.Error(new {Message = "Customer retrieved", Method = nameof(GetCustomer), Customer = customerResult.Value.ToJson()}.ToJson());
        
        return Result.Ok(customerResult.Value);
    }
}