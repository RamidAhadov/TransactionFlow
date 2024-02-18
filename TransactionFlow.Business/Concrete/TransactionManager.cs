using System.ComponentModel;
using AutoMapper;
using FluentResults;
using TransactionFlow.Business.Abstraction;
using TransactionFlow.Business.Models;
using TransactionFlow.Core.Constants;
using TransactionFlow.DataAccess.Abstraction;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.Business.Concrete;

public class TransactionManager:ITransactionManager
{
    private ITransactionDal _transactionDal;
    private ICustomerDal _customerDal;
    private IMapper _mapper;

    public TransactionManager(ITransactionDal transactionDal, IMapper mapper, ICustomerDal customerDal)
    {
        _transactionDal = transactionDal;
        _mapper = mapper;
        _customerDal = customerDal;
    }

    public Result<List<TransactionModel>> GetTransactions(int count)
    {
        if (count < 0)
        {
            return Result.Fail(ErrorMessages.IncorrectFormat);
        }

        try
        {
            return Result.Ok(count == 0 ? _mapper.Map<List<TransactionModel>>(_transactionDal.GetList()) : _mapper.Map<List<TransactionModel>>(_transactionDal.GetTransactions(count)));
        }
        catch (Exception)
        {
            return Result.Fail(ErrorMessages.CannotGetTransactions);
        }
    }

    public Result<List<TransactionModel>> GetSentAccountTransactions(int accountId, int count)
    {
        if (count < 0)
        {
            return Result.Fail(ErrorMessages.IncorrectFormat);
        }

        if (accountId <= 0)
        {
            return Result.Fail(ErrorMessages.IncorrectFormat);
        }
        
        return SentAccountTransactions(accountId, count);
    }

    public Result<List<TransactionModel>> GetReceivedAccountTransactions(int accountId, int count)
    {
        if (count < 0)
        {
            return Result.Fail(ErrorMessages.IncorrectFormat);
        }

        if (accountId <= 0)
        {
            return Result.Fail(ErrorMessages.IncorrectFormat);
        }
        
        return ReceivedAccountTransactions(accountId, count);
    }

    public Result<List<TransactionModel>> GetSentTransactions(List<CustomerAccountModel> accounts, int count)
    {
        if (count < 0)
        {
            return Result.Fail(ErrorMessages.IncorrectFormat);
        }
        
        var allTransactions = new List<TransactionModel>();
        foreach (var customerAccount in accounts)
        {
            allTransactions.AddRange(SentAccountTransactions(customerAccount.AccountId,count).Value);
        }
        
        return Result.Ok(allTransactions);
    }

    public Result<List<TransactionModel>> GetReceivedTransactions(List<CustomerAccountModel> accounts, int count)
    {
        if (count < 0)
        {
            return Result.Fail(ErrorMessages.IncorrectFormat);
        }
        
        var allTransactions = new List<TransactionModel>();
        foreach (var customerAccount in accounts)
        {
            allTransactions.AddRange(ReceivedAccountTransactions(customerAccount.AccountId,count).Value);
        }
        
        return Result.Ok(allTransactions);
    }

    [Description(description:"Count describes last *count transaction(s)")]
    public Result<List<Transaction>> GetTransactions(Customer customer,int? count)
    { 
        if (customer == null)
        {
            return Result.Fail(ErrorMessages.NullObjectEntered);
        }
            
        var list = _transactionDal.GetTransactions(customer,count);
        if (list == null)
        {
            return Result.Fail(ErrorMessages.ObjectNotFound);
        }

        if (list.Count == 0)
        {
            return Result.Fail(InfoMessages.ZeroTransactionFound);
        }
            
        return Result.Ok(list);
    }

    public Result<List<Transaction>> GetSentTransactions(Customer customer, int? count)
    {
        if (customer == null)
        {
            return Result.Fail(ErrorMessages.NullObjectEntered);
        }
            
        var list = _transactionDal.GetSentTransactions(customer,count);
        if (list == null)
        {
            return Result.Fail(ErrorMessages.ObjectNotFound);
        }

        if (list.Count == 0)
        {
            return Result.Fail(InfoMessages.ZeroTransactionFound);
        }
            
        return Result.Ok(list);
    }

    public Result<List<Transaction>> GetReceivedTransactions(Customer customer, int? count)
    {
        if (customer == null)
        {
            return Result.Fail(ErrorMessages.NullObjectEntered);
        }
            
        var list = _transactionDal.GetReceivedTransactions(customer,count);
        if (list == null)
        {
            return Result.Fail(ErrorMessages.ObjectNotFound);
        }

        if (list.Count == 0)
        {
            return Result.Fail(InfoMessages.ZeroTransactionFound);
        }
            
        return Result.Ok(list);
    }

    public async Task<Result<TransactionModel>> CreateTransactionAsync(TransferParticipants participants, decimal amount, decimal serviceFee, short transactionType)
    {
        var transactionModel = new TransactionModel
        {
            SenderId = participants.SenderId,
            SenderAccountId = participants.SenderAccountId,
            ReceiverId = participants.ReceiverId,
            ReceiverAccountId = participants.ReceiverAccountId,
            TransactionAmount = amount,
            ServiceFee = serviceFee,
            TransactionStatus = false,
            TransactionType = transactionType
        };
        try
        {
            return Result.Ok<TransactionModel>(_mapper.Map<TransactionModel>(
                await _transactionDal.AddAsync(_mapper.Map<Transaction>(transactionModel))));
        }
        catch (Exception)
        {
            return Result.Fail(ErrorMessages.TransactionNotCreated);
        }
    }

    public async Task<Result> ChangeTransactionStatus(Transaction transaction)
    {
        try
        {
            transaction.TransactionStatus = true;
            await _transactionDal.UpdateAsync(transaction);
            return Result.Ok();
        }
        catch (Exception e)
        {
            return Result.Fail(ErrorMessages.TransactionStatusError);
        }
    }
    
    
    private Result<List<TransactionModel>> ReceivedAccountTransactions(int accountId, int count)
    {
        try
        {
            var transactions =
                _mapper.Map<List<TransactionModel>>(_transactionDal.GetReceivedTransactions(accountId, count));
            if (transactions.Count == 0)
            {
                return Result.Ok();
            }
            
            return Result.Ok(transactions);
        }
        catch (Exception exception)
        {
            return Result.Fail(exception.Message);
        }
    }
    
    private Result<List<TransactionModel>> SentAccountTransactions(int accountId, int count)
    {
        try
        {
            var transactions =
                _mapper.Map<List<TransactionModel>>(_transactionDal.GetSentTransactions(accountId, count));
            if (transactions.Count == 0)
            {
                return Result.Ok();
            }
            
            return Result.Ok(transactions);
        }
        catch (Exception exception)
        {
            return Result.Fail(exception.Message);
        }
    }
}