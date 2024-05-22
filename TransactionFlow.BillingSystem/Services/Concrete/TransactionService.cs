using System.Diagnostics;
using AutoMapper;
using FluentResults;
using NLog;
using NuGet.Protocol;
using TransactionFlow.BillingSystem.Services.Abstraction;
using TransactionFlow.Business.Abstraction;
using TransactionFlow.Business.Models;
using TransactionFlow.Core.Constants;

namespace TransactionFlow.BillingSystem.Services.Concrete;

public class TransactionService:ITransactionService
{
    private readonly ITransactionManager _transactionManager;
    private readonly ICustomerManager _customerManager;
    private readonly IMapper _mapper;
    private readonly Logger _logger;

    public TransactionService(
        ITransactionManager transactionManager, 
        ICustomerManager customerManager, 
        IMapper mapper)
    {
        _transactionManager = transactionManager;
        _customerManager = customerManager;
        _mapper = mapper;
        _logger = LogManager.GetLogger("TransactionServiceLogger");
    }

    public Result<List<TransactionModel>> GetTransactions(int count)
    {
        var sw = Stopwatch.StartNew();
        var transactionResult = _transactionManager.GetTransactions(count);
        if (transactionResult.IsFailed)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Message = transactionResult.Errors,Method = nameof(GetTransactions)}.ToJson());
            
            return Result.Fail(transactionResult.Errors);
        }
        _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Message = "Transactions retrieved.",Method = nameof(GetTransactions), Transactions = transactionResult.Value.ToJson()}.ToJson());
        
        return Result.Ok(transactionResult.Value);
    }

    public Result<List<TransactionModel>> GetAccountTransactions(int accountId, int count)
    {
        var sw = Stopwatch.StartNew();
        var allTransactions = new List<TransactionModel>();
        var sentTransactionsResult = _transactionManager.GetSentAccountTransactions(accountId, count);
        if (sentTransactionsResult.IsFailed)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Message = sentTransactionsResult.Errors, Method = nameof(GetAccountTransactions), AccountId = accountId}.ToJson());
            
            return Result.Fail(sentTransactionsResult.Errors);
        }
        var receivedTransactionsResult = _transactionManager.GetReceivedAccountTransactions(accountId, count);
        if (receivedTransactionsResult.IsFailed)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Message = receivedTransactionsResult.Errors, Method = nameof(GetAccountTransactions), AccountId = accountId}.ToJson());
            
            return Result.Fail(receivedTransactionsResult.Errors);
        }
        
        if (sentTransactionsResult.Value.Count != 0)
        {
            allTransactions.AddRange(sentTransactionsResult.Value);
        }

        if (receivedTransactionsResult.Value.Count != 0)
        {
            allTransactions.AddRange(receivedTransactionsResult.Value);
        }
        
        if (allTransactions.Count == 0)
        {
            _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Message = "There are not any transactions.", Method = nameof(GetAccountTransactions), AccountId = accountId}.ToJson());
            
            return Result.Fail(InfoMessages.ZeroTransactionFound);
        }
        
        allTransactions.Sort((a,b)=>b.Id.CompareTo(a.Id));
        RemoveDuplicates(ref allTransactions);
        _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Message = "Transactions retrieved.", Method = nameof(GetAccountTransactions), AccountId = accountId, Transactions = allTransactions.ToJson()}.ToJson());
        
        return Result.Ok(allTransactions);
    }

    public Result<List<TransactionModel>> GetSentAccountTransactions(int accountId, int count)
    {
        var sw = Stopwatch.StartNew();
        var transactionsResult = _transactionManager.GetSentAccountTransactions(accountId, count);
        if (transactionsResult.Value.Count == 0)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Message = transactionsResult.Errors, Method = nameof(GetSentAccountTransactions), AccountId = accountId}.ToJson());
            return Result.Fail(InfoMessages.ZeroTransactionFound);
        }
        
        if (transactionsResult.IsFailed)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Message = transactionsResult.Errors, Method = nameof(GetSentAccountTransactions), AccountId = accountId}.ToJson());
            
            return Result.Fail(transactionsResult.Errors);
        }
        _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Message = "Transactions retrieved.", Method = nameof(GetSentAccountTransactions), AccountId = accountId, Transactions = transactionsResult.Value.ToJson()}.ToJson());
        
        return Result.Ok(transactionsResult.Value);
    }

    public Result<List<TransactionModel>> GetReceivedAccountTransactions(int accountId, int count)
    {
        var sw = Stopwatch.StartNew();
        var transactionsResult = _transactionManager.GetReceivedAccountTransactions(accountId, count);
        if (transactionsResult.Value.Count == 0)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Message = transactionsResult.Errors, Method = nameof(GetReceivedAccountTransactions), AccountId = accountId}.ToJson());
            
            return Result.Fail(InfoMessages.ZeroTransactionFound);
        }
        
        if (transactionsResult.IsFailed)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Message = transactionsResult.Errors, Method = nameof(GetReceivedAccountTransactions), AccountId = accountId}.ToJson());
            
            return Result.Fail(transactionsResult.Errors);
        }
        _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Message = "Transactions retrieved", Method = nameof(GetReceivedAccountTransactions), AccountId = accountId, Transactions = transactionsResult.Value.ToJson()}.ToJson());
        
        return Result.Ok(transactionsResult.Value);
    }

    public Result<List<TransactionModel>> GetCustomerTransactions(int customerId, int count)
    {
        var sw = Stopwatch.StartNew();
        var customerResult = _customerManager.GetCustomerWithAccounts(customerId);
        if (customerResult.IsFailed)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Message = customerResult.Errors, Method = nameof(GetCustomerTransactions), CustomerId = customerId}.ToJson());
            
            return Result.Fail(customerResult.Errors);
        }
        
        var allTransactions = new List<TransactionModel>();
        var sentTransactionsResult = _transactionManager.GetSentTransactions(customerResult.Value.Accounts, count);
        if (sentTransactionsResult.IsFailed)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Message = sentTransactionsResult.Errors, Method = nameof(GetCustomerTransactions), CustomerId = customerId}.ToJson());

            return Result.Fail(sentTransactionsResult.Errors);
        }
        var receivedTransactionsResult = _transactionManager.GetReceivedTransactions(customerResult.Value.Accounts, count);
        if (receivedTransactionsResult.IsFailed)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Message = receivedTransactionsResult.Errors, Method = nameof(GetCustomerTransactions), CustomerId = customerId}.ToJson());

            return Result.Fail(receivedTransactionsResult.Errors);
        }

        if (sentTransactionsResult.Value.Count != 0)
        {
            allTransactions.AddRange(sentTransactionsResult.Value);
        }

        if (receivedTransactionsResult.Value.Count != 0)
        {
            allTransactions.AddRange(receivedTransactionsResult.Value);
        }
        
        if (allTransactions.Count == 0)
        {
            _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Message = "There are not any transactions.", Method = nameof(GetCustomerTransactions), CustomerId = customerId}.ToJson());

            return Result.Fail(InfoMessages.ZeroTransactionFound);
        }
        
        allTransactions.Sort((a,b)=>b.Id.CompareTo(a.Id));
        RemoveDuplicates(ref allTransactions);
        _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Message = "Transactions retrieved", Method = nameof(GetCustomerTransactions), CustomerId = customerId, Transactions = allTransactions.ToJson()}.ToJson());
        
        return Result.Ok(allTransactions);
    }

    public Result<List<TransactionModel>> GetSentTransactions(int customerId, int count)
    {
        var sw = Stopwatch.StartNew();
        var customerResult = _customerManager.GetCustomerWithAccounts(customerId);
        if (customerResult.IsFailed)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Message = customerResult.Errors, Method = nameof(GetSentTransactions), CustomerId = customerId}.ToJson());

            return Result.Fail(customerResult.Errors);
        }

        var accountsModel = _mapper.Map<List<CustomerAccountModel>>(customerResult.Value.Accounts);
        var transactionsResult = _transactionManager.GetSentTransactions(accountsModel, count);
        if (transactionsResult.Value.Count == 0)
        {
            _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Message = "There are not any transactions.", Method = nameof(GetSentTransactions), CustomerId = customerId}.ToJson());

            return Result.Fail(InfoMessages.ZeroTransactionFound);
        }
        
        if (transactionsResult.IsFailed)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Message = transactionsResult.Errors, Method = nameof(GetSentTransactions), CustomerId = customerId}.ToJson());

            return Result.Fail(transactionsResult.Errors);
        }
        _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Message = "Transactions retrieved.", Method = nameof(GetSentTransactions), CustomerId = customerId, Transactions = transactionsResult.Value.ToJson()}.ToJson());
        
        return Result.Ok(transactionsResult.Value);
    }

    public Result<List<TransactionModel>> GetReceivedTransactions(int customerId, int count)
    {
        var sw = Stopwatch.StartNew();
        var customerResult = _customerManager.GetCustomerWithAccounts(customerId);
        if (customerResult.IsFailed)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Message = customerResult.Errors, Method = nameof(GetReceivedTransactions), CustomerId = customerId}.ToJson());

            return Result.Fail(customerResult.Errors);
        }

        var accountsModel = _mapper.Map<List<CustomerAccountModel>>(customerResult.Value.Accounts);
        var transactionsResult = _transactionManager.GetReceivedTransactions(accountsModel, count);
        if (transactionsResult.Value.Count == 0)
        {
            _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Message = "There are not any transactions.", Method = nameof(GetReceivedTransactions), CustomerId = customerId}.ToJson());

            return Result.Fail(InfoMessages.ZeroTransactionFound);
        }
        
        if (transactionsResult.IsFailed)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Message = transactionsResult.Errors, Method = nameof(GetReceivedTransactions), CustomerId = customerId}.ToJson());

            return Result.Fail(transactionsResult.Errors);
        }
        _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Message = "Transactions retrieved.", Method = nameof(GetReceivedTransactions), CustomerId = customerId, Transactions = transactionsResult.Value.ToJson()}.ToJson());
        
        return Result.Ok(transactionsResult.Value);
    }

    private void RemoveDuplicates(ref List<TransactionModel> transactions)
    {
        HashSet<int> seen = new HashSet<int>();
        List<TransactionModel> uniqueTransactions = new List<TransactionModel>();

        foreach (var item in transactions)
        {
            if (seen.Add(item.Id))
            {
                uniqueTransactions.Add(item);
            }
        }

        transactions = uniqueTransactions;
    }
}