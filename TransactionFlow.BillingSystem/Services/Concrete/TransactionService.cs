using AutoMapper;
using FluentResults;
using TransactionFlow.BillingSystem.Services.Abstraction;
using TransactionFlow.Business.Abstraction;
using TransactionFlow.Business.Models;

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

    public Result<List<TransactionModel>> GetSentAccountTransactions(int accountId, int count)
    {
        var transactionsResult = _transactionManager.GetSentAccountTransactions(accountId, count);
        if (transactionsResult.IsFailed)
        {
            return Result.Fail(transactionsResult.Errors);
        }
        
        return Result.Ok(transactionsResult.Value);
    }

    public Result<List<TransactionModel>> GetReceivedAccountTransactions(int accountId, int count)
    {
        var transactionsResult = _transactionManager.GetReceivedAccountTransactions(accountId, count);
        if (transactionsResult.IsFailed)
        {
            return Result.Fail(transactionsResult.Errors);
        }
        
        return Result.Ok(transactionsResult.Value);
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
        if (transactionsResult.IsFailed)
        {
            return Result.Fail(transactionsResult.Errors);
        }
        
        return Result.Ok(transactionsResult.Value);
    }
}