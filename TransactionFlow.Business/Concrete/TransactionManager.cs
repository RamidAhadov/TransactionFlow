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
    private IMapper _mapper;

    public TransactionManager(ITransactionDal transactionDal, IMapper mapper)
    {
        _transactionDal = transactionDal;
        _mapper = mapper;
    }

    [Description(description:"Count describes last *count transaction(s)")]
    public Result<List<Transaction>> GetTransactions(Customer customer,int? count)
    {
        //_log.Info("This is a log message.");   
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
            //TODO - Error
            return Result.Ok<TransactionModel>(_mapper.Map<TransactionModel>(
                await _transactionDal.AddAsync(_mapper.Map<Transaction>(transactionModel))));
        }
        catch (Exception e)
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
}