using AutoMapper;
using FluentResults;
using TransactionFlow.BillingSystem.Services.Abstraction;
using TransactionFlow.Business.Abstraction;
using TransactionFlow.Business.Models;
using TransactionFlow.Core.Constants;

namespace TransactionFlow.BillingSystem.Services.Concrete;

public class TransactionService:ITransactionService
{
    private ITransactionManager _transactionManager;
    private ICustomerManager _customerManager;
    private IMapper _mapper;

    public TransactionService(
        ITransactionManager transactionManager, 
        ICustomerManager customerManager, 
        IMapper mapper)
    {
        _transactionManager = transactionManager;
        _customerManager = customerManager;
        _mapper = mapper;
    }

    public Result<List<TransactionModel>> GetTransactions(int count)
    {
        var transactionResult = _transactionManager.GetTransactions(count);
        if (transactionResult.IsFailed)
        {
            return Result.Fail(transactionResult.Errors);
        }
        
        return Result.Ok(transactionResult.Value);
    }

    public Result<List<TransactionModel>> GetAccountTransactions(int accountId, int count)
    {
        var allTransactions = new List<TransactionModel>();
        var sentTransactionsResult = _transactionManager.GetSentAccountTransactions(accountId, count);
        if (sentTransactionsResult.IsFailed)
        {
            return Result.Fail(sentTransactionsResult.Errors);
        }
        var receivedTransactionsResult = _transactionManager.GetReceivedAccountTransactions(accountId, count);
        if (receivedTransactionsResult.IsFailed)
        {
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
            return Result.Fail(InfoMessages.ZeroTransactionFound);
        }
        
        allTransactions.Sort((a,b)=>b.Id.CompareTo(a.Id));
        RemoveDuplicates(ref allTransactions);
        
        return Result.Ok(allTransactions);
    }

    public Result<List<TransactionModel>> GetSentAccountTransactions(int accountId, int count)
    {
        var transactionsResult = _transactionManager.GetSentAccountTransactions(accountId, count);
        if (transactionsResult.Value.Count == 0)
        {
            return Result.Fail(InfoMessages.ZeroTransactionFound);
        }
        
        if (transactionsResult.IsFailed)
        {
            return Result.Fail(transactionsResult.Errors);
        }
        
        return Result.Ok(transactionsResult.Value);
    }

    public Result<List<TransactionModel>> GetReceivedAccountTransactions(int accountId, int count)
    {
        var transactionsResult = _transactionManager.GetReceivedAccountTransactions(accountId, count);
        if (transactionsResult.Value.Count == 0)
        {
            return Result.Fail(InfoMessages.ZeroTransactionFound);
        }
        
        if (transactionsResult.IsFailed)
        {
            return Result.Fail(transactionsResult.Errors);
        }
        
        return Result.Ok(transactionsResult.Value);
    }

    public Result<List<TransactionModel>> GetCustomerTransactions(int customerId, int count)
    {
        var customerResult = _customerManager.GetCustomerWithAccounts(customerId);
        if (customerResult.IsFailed)
        {
            return Result.Fail(customerResult.Errors);
        }
        
        var allTransactions = new List<TransactionModel>();
        var sentTransactionsResult = _transactionManager.GetSentTransactions(customerResult.Value.Accounts, count);
        if (sentTransactionsResult.IsFailed)
        {
            return Result.Fail(sentTransactionsResult.Errors);
        }
        var receivedTransactionsResult = _transactionManager.GetReceivedTransactions(customerResult.Value.Accounts, count);
        if (receivedTransactionsResult.IsFailed)
        {
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
            return Result.Fail(InfoMessages.ZeroTransactionFound);
        }
        
        allTransactions.Sort((a,b)=>b.Id.CompareTo(a.Id));
        RemoveDuplicates(ref allTransactions);
        
        return Result.Ok(allTransactions);
    }

    public Result<List<TransactionModel>> GetSentTransactions(int customerId, int count)
    {
        var customerResult = _customerManager.GetCustomerWithAccounts(customerId);
        if (customerResult.IsFailed)
        {
            return Result.Fail(customerResult.Errors);
        }

        var accountsModel = _mapper.Map<List<CustomerAccountModel>>(customerResult.Value.Accounts);
        var transactionsResult = _transactionManager.GetSentTransactions(accountsModel, count);
        if (transactionsResult.Value.Count == 0)
        {
            return Result.Fail(InfoMessages.ZeroTransactionFound);
        }
        
        if (transactionsResult.IsFailed)
        {
            return Result.Fail(transactionsResult.Errors);
        }
        
        return Result.Ok(transactionsResult.Value);
    }

    public Result<List<TransactionModel>> GetReceivedTransactions(int customerId, int count)
    {
        var customerResult = _customerManager.GetCustomerWithAccounts(customerId);
        if (customerResult.IsFailed)
        {
            return Result.Fail(customerResult.Errors);
        }

        var accountsModel = _mapper.Map<List<CustomerAccountModel>>(customerResult.Value.Accounts);
        var transactionsResult = _transactionManager.GetReceivedTransactions(accountsModel, count);
        if (transactionsResult.Value.Count == 0)
        {
            return Result.Fail(InfoMessages.ZeroTransactionFound);
        }
        
        if (transactionsResult.IsFailed)
        {
            return Result.Fail(transactionsResult.Errors);
        }
        
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