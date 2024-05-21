using System.Diagnostics;
using AutoMapper;
using FluentResults;
using NLog;
using NuGet.Protocol;
using TransactionFlow.Business.Abstraction;
using TransactionFlow.Business.Models;
using TransactionFlow.Core.Constants;
using TransactionFlow.DataAccess.Abstraction;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.Business.Concrete;

public class CustomerAccountManager:IAccountManager
{
    private readonly ICustomerAccountDal _customerAccountDal;
    private readonly ICustomerDal _customerDal;
    private readonly IMapper _mapper;
    private readonly Logger _logger;

    public CustomerAccountManager(ICustomerAccountDal customerAccountDal, ICustomerDal customerDal, IMapper mapper)
    {
        _customerAccountDal = customerAccountDal;
        _customerDal = customerDal;
        _mapper = mapper;
        _logger = LogManager.GetLogger("CustomerAccountManagerLogger");
    }

    public Result CreateAccount(CustomerModel customerModel)
    {
        var sw = Stopwatch.StartNew();
        var accountModel = new CustomerAccountModel
        {
            CustomerId = customerModel.Id,
            Balance = 100,
            IsActive = true,
            IsMain = !HasAccount(customerModel),
            CreatedDate = DateTime.Now
        };

        try
        {
            var newAccount = _mapper.Map<CustomerAccount>(accountModel);
            _customerAccountDal.Add(newAccount);
            _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(CreateAccount), Message = "Customer account created.", Account = newAccount.ToJson() }.ToJson());
            
            return Result.Ok();
        }
        catch (Exception exception)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(CreateAccount), Message = exception.InnerException?.Message ?? exception.Message}.ToJson());
            
            return Result.Fail(ErrorMessages.AccountNotCreated);
        }
    }

    public async Task<Result> CreateAccountAsync(CustomerModel customerModel)
    {
        var sw = Stopwatch.StartNew();
        var accountModel = new CustomerAccountModel
        {
            CustomerId = customerModel.Id,
            Balance = 100,
            IsActive = true,
            IsMain = !await HasAccountAsync(customerModel),
            CreatedDate = DateTime.Now
        };

        try
        {
            var newAccount = _mapper.Map<CustomerAccount>(accountModel);
            await _customerAccountDal.AddAsync(newAccount);
            _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(CreateAccountAsync), Message = "Customer account created.", Account = newAccount.ToJson()}.ToJson());

            return Result.Ok();
        }
        catch (Exception exception)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(CreateAccountAsync), Message = exception.InnerException?.Message ?? exception.Message}.ToJson());
            
            return Result.Fail(ErrorMessages.AccountNotCreated);
        }
    }

    public async Task<Result<List<CustomerAccountModel>>> GetAccountsAsync(int customerId)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var accounts = await GetAccountListAsync(customerId);
            _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(GetAccountsAsync), Message = "Customer account list retrieved.", accounts.Count }.ToJson());

            return Result.Ok(accounts);
        }
        catch (Exception exception)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(GetAccountsAsync), Message = exception.InnerException?.Message ?? exception.Message}.ToJson());
            
            return Result.Fail(ErrorMessages.AccountsNotFound);
        }
    }

    
    public Result<CustomerAccountModel> GetMainAccount(int customerId)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var mainAccount = _mapper.Map<CustomerAccountModel>(_customerDal.GetCustomerWithAccounts(customerId)
                .CustomerAccounts.FirstOrDefault(ca => ca.IsMain));
            _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(GetMainAccount), Message = "Customer's main account retrieved.", Account = mainAccount.ToJson() }.ToJson());

            return Result.Ok(mainAccount);
        }
        catch (Exception exception)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(GetMainAccount), Message = exception.InnerException?.Message ?? exception.Message, CustomerId = customerId}.ToJson());
            
            return Result.Fail(ErrorMessages.AccountNotFound);
        }
    }

    public async Task<Result<List<CustomerAccountModel>>> GetAccountsByAccountAsync(int accountId)
    {
        var sw = Stopwatch.StartNew();
        var customerId = _customerAccountDal.Get(ca => ca.AccountId == accountId).CustomerId;
        try
        {
            var accounts = await GetAccountListAsync(customerId);
            if (accounts == null || accounts.Count == 0)
            {
                _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(GetAccountsByAccountAsync), Message = ErrorMessages.AccountsNotFound}.ToJson());
                
                return Result.Fail(ErrorMessages.AccountNotFound);
            }
            
            _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(GetAccountsByAccountAsync), Message = "Customer's accounts retrieved.", accounts.Count}.ToJson());
            
            return Result.Ok(accounts);
        }
        catch (Exception exception)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(GetAccountsByAccountAsync), Message = exception.InnerException?.Message ?? exception.Message}.ToJson());
            
            return Result.Fail(ErrorMessages.AccountsNotFound);
        }
    }

    public async Task<Result> DeleteAccountAsync(CustomerAccountModel accountModel)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            await _customerAccountDal.DeleteAsync(_mapper.Map<CustomerAccount>(accountModel));
            _logger.Info(new { Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(DeleteAccountAsync), Message = "Customer's account deleted.", accountModel.CustomerId, accountModel.AccountId }.ToJson());
            
            return Result.Ok();
        }
        catch (Exception exception)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(DeleteAccountAsync), Message = exception.InnerException?.Message ?? exception.Message}.ToJson());
            return Result.Fail(ErrorMessages.AccountNotDeleted);
        }
    }

    public Result DeleteAccount(List<CustomerAccountModel> accountModels)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            _customerAccountDal.DeleteRange(_mapper.Map<List<CustomerAccount>>(accountModels));
            _logger.Info(new { Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(DeleteAccount), Message = "Customer's account deleted.", Deleted = accountModels.Count }.ToJson());
            
            return Result.Ok();
        }
        catch (Exception exception)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(DeleteAccount), Message = exception.InnerException?.Message ?? exception.Message}.ToJson());
            return Result.Fail(ErrorMessages.AccountNotDeleted);
        }
    }

    public Result<CustomerAccountModel> GetAccount(int accountId)
    {
        var sw = Stopwatch.StartNew();
        if (accountId <= 0)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(GetAccount), Message = "Given ID is less than zero."}.ToJson());
            
            return Result.Fail(ErrorMessages.IndexOutOfTheRange);
        }

        CustomerAccountModel? accountModel;
        try
        {
            accountModel = _mapper.Map<CustomerAccountModel>( _customerAccountDal.Get(ca => ca.AccountId == accountId));
        }
        catch (Exception exception)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(GetAccount), Message = exception.InnerException?.Message ?? exception.Message}.ToJson());
            
            return Result.Fail(ErrorMessages.AccountNotFound);
        }

        if (accountModel == null)
        {
            _logger.Warn(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(GetAccount), Message = ErrorMessages.AccountNotFound}.ToJson());
            
            return Result.Fail(ErrorMessages.AccountNotFound);
        }

        _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(GetAccount), Message = "Account retrieved.", Account = accountModel.ToJson()}.ToJson());
        
        return Result.Ok(accountModel);
    }

    public async Task<Result> ChangeMainAccountAsync(CustomerAccountModel customerAccountModel)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            await _customerAccountDal.ChangeMainAccountAsync(customerAccountModel.CustomerId);
            _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(ChangeMainAccountAsync), Message = $"Customer's main account changed to account with ID {customerAccountModel.AccountId}."}.ToJson());
            
            return Result.Ok();
        }
        catch (Exception exception)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(ChangeMainAccountAsync), Message = exception.InnerException?.Message ?? exception.Message}.ToJson());
            
            return Result.Fail(ErrorMessages.OperationFailed);
        }
    }

    public async Task<Result<CustomerAccountModel>> TransferToMainAsync(TransactionModel transactionModel)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var account = await _customerAccountDal.TransferAsync(_mapper.Map<TransactionDetails>(transactionModel));
            _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(TransferToMainAsync), Message = $"{transactionModel.TransactionAmount} $ transferred to account with ID: {transactionModel.ReceiverAccountId}."}.ToJson());
            
            return Result.Ok(_mapper.Map<CustomerAccountModel>(account));
        }
        catch (Exception exception)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(TransferToMainAsync), Message = exception.InnerException?.Message ?? exception.Message}.ToJson());
            
            return Result.Fail(ErrorMessages.TransferFailed);
        }
    }

    public async Task<Result> DeactivateAccountAsync(CustomerAccountModel accountModel)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            accountModel.IsActive = false;
            await _customerAccountDal.UpdateAsync(_mapper.Map<CustomerAccount>(accountModel));
            _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(DeactivateAccountAsync), Message = $"Deactivated account with ID: {accountModel.AccountId}"}.ToJson());
            
            return Result.Ok();
        }
        catch (Exception exception)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(DeactivateAccountAsync), Message = exception.InnerException?.Message ?? exception.Message}.ToJson());
            
            return Result.Fail(ErrorMessages.OperationFailed);
        }
    }

    public async Task<Result> DeactivateAccountAsync(List<CustomerAccountModel> accountModels)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            await _customerAccountDal.UpdateRangeAsync(_mapper.Map<List<CustomerAccount>>(accountModels));
            _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(DeactivateAccountAsync), Message = $"{accountModels.Count} accounts deactivated."}.ToJson());
            
            return Result.Ok();
        }
        catch (Exception exception)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(DeactivateAccountAsync), Message = exception.InnerException?.Message ?? exception.Message}.ToJson());
            
            return Result.Fail(ErrorMessages.OperationFailed);
        }
    }

    public async Task<Result> ActivateAccountAsync(CustomerAccountModel accountModel)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            accountModel.IsActive = true;
            await _customerAccountDal.UpdateAsync(_mapper.Map<CustomerAccount>(accountModel));
            _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(ActivateAccountAsync), Message = $"Activated account with ID: {accountModel.AccountId}."}.ToJson());
            
            return Result.Ok();
        }
        catch (Exception exception)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(ActivateAccountAsync), Message = exception.InnerException?.Message ?? exception.Message}.ToJson());
            
            return Result.Fail(ErrorMessages.OperationFailed);
        }
    }

    public async Task<Result> TransferMoneyAsync(TransactionModel transactionModel)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            await _customerAccountDal.TransferAsync(_mapper.Map<TransactionDetails>(transactionModel));
            _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(TransferMoneyAsync), Message = $"Transferred between {transactionModel.SenderAccountId} and {transactionModel.ReceiverAccountId}."}.ToJson());
            
            return Result.Ok();
        }
        catch (Exception exception)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(TransferMoneyAsync), Message = exception.InnerException?.Message ?? exception.Message}.ToJson());
            
            return Result.Fail(ErrorMessages.TransferFailed);
        }
    }

    private bool HasAccount(CustomerModel customerModel)
    {
        var accountList = GetAccountList(customerModel);

        return accountList.Count != 0;
    }
    
    private async Task<bool> HasAccountAsync(CustomerModel customerModel)
    {
        var accountList = await GetAccountListAsync(customerModel);

        return accountList.Count != 0;
    }

    private async Task<List<CustomerAccountModel>> GetAccountListAsync(CustomerModel customerModel)
    {
        return _mapper.Map<List<CustomerAccountModel>>(
            await _customerDal.GetAccountsAsync(_mapper.Map<Customer>(customerModel)));
    }
    
    private async Task<List<CustomerAccountModel>> GetAccountListAsync(int customerId)
    {
        return _mapper.Map<List<CustomerAccountModel>>(await _customerDal.GetAccountsAsync(customerId));
    }

    private List<CustomerAccountModel> GetAccountList(CustomerModel customerModel)
    {
        return _mapper.Map<List<CustomerAccountModel>>(_customerDal.GetAccounts(_mapper.Map<Customer>(customerModel)));
    }
}