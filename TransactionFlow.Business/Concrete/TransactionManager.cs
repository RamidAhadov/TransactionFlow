using System.ComponentModel;
using System.Diagnostics;
using AutoMapper;
using FluentResults;
using Microsoft.Identity.Client;
using NLog;
using NuGet.Protocol;
using TransactionFlow.Business.Abstraction;
using TransactionFlow.Business.Models;
using TransactionFlow.Core.Constants;
using TransactionFlow.DataAccess.Abstraction;
using TransactionFlow.Entities.Concrete;
using Logger = NLog.Logger;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace TransactionFlow.Business.Concrete;

public class TransactionManager:ITransactionManager
{
    private readonly ITransactionDal _transactionDal;
    private readonly IMapper _mapper;
    private readonly Logger _logger;

    public TransactionManager(ITransactionDal transactionDal, IMapper mapper)
    {
        _transactionDal = transactionDal;
        _mapper = mapper;
        _logger = LogManager.GetLogger("TransactionManagerLogger");
    }

    public Result<List<TransactionModel>> GetTransactions(int count)
    {
        var sw = Stopwatch.StartNew();
        if (count < 0)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(GetTransactions), Message = "Count must not be negative number.", Count = count}.ToJson());
            
            return Result.Fail(ErrorMessages.IncorrectFormat);
        }

        try
        {
            var transactions = count == 0
                ? _mapper.Map<List<TransactionModel>>(_transactionDal.GetList())
                : _mapper.Map<List<TransactionModel>>(_transactionDal.GetTransactions(count));
            _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(GetTransactions), Message = "Transactions retrieved.", transactions.Count}.ToJson());
            
            return Result.Ok(transactions);
        }
        catch (Exception exception)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(GetTransactions), Message = exception.InnerException?.Message ?? exception.Message}.ToJson());
            
            return Result.Fail(ErrorMessages.CannotGetTransactions);
        }
    }

    public Result<List<TransactionModel>> GetSentAccountTransactions(int accountId, int count)
    {
        var sw = Stopwatch.StartNew();
        if (count < 0)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(GetSentAccountTransactions), Message = "Count must not be negative number.", Count = count}.ToJson());
            
            return Result.Fail(ErrorMessages.IncorrectFormat);
        }

        if (accountId <= 0)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(GetSentAccountTransactions), Message = "Account ID must be greater than zero.", AccountId = accountId}.ToJson());
            
            return Result.Fail(ErrorMessages.IncorrectFormat);
        }
        
        return SentAccountTransactions(accountId, count);
    }

    public Result<List<TransactionModel>> GetReceivedAccountTransactions(int accountId, int count)
    {
        var sw = Stopwatch.StartNew();
        if (count < 0)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(GetReceivedAccountTransactions), Message = "Count must not be negative number.", Count = count}.ToJson());
            
            return Result.Fail(ErrorMessages.IncorrectFormat);
        }

        if (accountId <= 0)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(GetReceivedAccountTransactions), Message = "Account ID must be greater than zero.", AccountId = accountId}.ToJson());
            
            return Result.Fail(ErrorMessages.IncorrectFormat);
        }
        
        return ReceivedAccountTransactions(accountId, count);
    }

    public Result<List<TransactionModel>> GetSentTransactions(List<CustomerAccountModel> accounts, int count)
    {
        var sw = Stopwatch.StartNew();
        if (count < 0)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(GetSentTransactions), Message = "Count must not be negative number.", Count = count}.ToJson());
            
            return Result.Fail(ErrorMessages.IncorrectFormat);
        }
        
        var allTransactions = new List<TransactionModel>();
        foreach (var customerAccount in accounts)
        {
            var accountTransactions = SentAccountTransactions(customerAccount.AccountId, count).Value;
            if (accountTransactions.Count != 0)
            {
                allTransactions.AddRange(accountTransactions);
            }
        }
        _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(GetSentTransactions), Message = "Transactions retrieved." ,allTransactions.Count}.ToJson());
        
        return Result.Ok(allTransactions);
    }

    public Result<List<TransactionModel>> GetReceivedTransactions(List<CustomerAccountModel> accounts, int count)
    {
        var sw = Stopwatch.StartNew();
        if (count < 0)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(GetReceivedTransactions), Message = "Count must not be negative number.", Count = count}.ToJson());
            
            return Result.Fail(ErrorMessages.IncorrectFormat);
        }
        
        var allTransactions = new List<TransactionModel>();
        foreach (var customerAccount in accounts)
        {
            var accountTransactions = ReceivedAccountTransactions(customerAccount.AccountId, count).Value;
            if (accountTransactions != null || accountTransactions.Count != 0)
            {
                allTransactions.AddRange(accountTransactions);
            }
        }
        _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(GetReceivedTransactions), Message = "Transactions retrieved." ,allTransactions.Count}.ToJson());
        
        return Result.Ok(allTransactions);
    }

    public async Task<Result<TransactionModel>> CreateTransactionAsync(TransferParticipants participants, decimal amount, decimal serviceFee, short transactionType)
    {
        var sw = Stopwatch.StartNew();
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
            var model = _mapper.Map<TransactionModel>(
                await _transactionDal.AddAsync(_mapper.Map<Transaction>(transactionModel)));
            _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(CreateTransactionAsync), Message = "Transactions created.",TransactionId =  model.Id}.ToJson());

            return Result.Ok<TransactionModel>(model);
        }
        catch (Exception exception)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(CreateTransactionAsync), Message = exception.InnerException?.Message ?? exception.Message}.ToJson());
            
            return Result.Fail(ErrorMessages.TransactionNotCreated);
        }
    }
    
    private Result<List<TransactionModel>> ReceivedAccountTransactions(int accountId, int count)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var transactions =
                _mapper.Map<List<TransactionModel>>(_transactionDal.GetReceivedTransactions(accountId, count));
            _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(ReceivedAccountTransactions), Message = "Transactions retrieved." ,transactions.Count, AccountId = accountId}.ToJson());
            
            return Result.Ok(transactions);
        }
        catch (Exception exception)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(ReceivedAccountTransactions), Message = exception.InnerException?.Message ?? exception.Message, AccountId = accountId}.ToJson());
            
            return Result.Fail(ErrorMessages.CannotGetTransactions);
        }
    }
    
    private Result<List<TransactionModel>> SentAccountTransactions(int accountId, int count)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var transactions =
                _mapper.Map<List<TransactionModel>>(_transactionDal.GetSentTransactions(accountId, count));
            _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(SentAccountTransactions), Message = "Transactions retrieved." ,transactions.Count, AccountId = accountId}.ToJson());
            
            return Result.Ok(transactions);
        }
        catch (Exception exception)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(SentAccountTransactions), Message = exception.InnerException?.Message ?? exception.Message, AccountId = accountId}.ToJson());
            
            return Result.Fail(ErrorMessages.CannotGetTransactions);
        }
    }
}