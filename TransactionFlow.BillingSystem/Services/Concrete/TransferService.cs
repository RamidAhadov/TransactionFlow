using FluentResults;
using NLog;
using NuGet.Protocol;
using TransactionFlow.BillingSystem.Models.Dtos;
using TransactionFlow.BillingSystem.Services.Abstraction;
using TransactionFlow.Business.Abstraction;
using TransactionFlow.Business.Models;
using TransactionFlow.Core.Constants;
#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace TransactionFlow.BillingSystem.Services.Concrete;

public class TransferService:ITransferService
{
    private readonly IAccountManager _accountManager;
    private readonly ITransactionManager _transactionManager;
    private readonly Logger _logger;

    public TransferService(IAccountManager accountManager, ITransactionManager transactionManager)
    {
        _accountManager = accountManager;
        _transactionManager = transactionManager;
        _logger = LogManager.GetLogger("TransferServiceLogger");
    }

    public async Task<Result> TransferMoneyAsync(TransferDto transferDto,TransferConditions conditions)
    {
        Result<List<CustomerAccountModel>>? senderAccountsResult;
        Result<List<CustomerAccountModel>>? receiverAccountsResult;
        
        (senderAccountsResult,receiverAccountsResult) = await FilterAccountsByCondition(transferDto,conditions);
        
        if (senderAccountsResult.IsFailed)
        {
            _logger.Error(new {Message = senderAccountsResult.Errors,Method = nameof(TransferMoneyAsync), TransferDetails = transferDto.ToJson()}.ToJson());
            
            return Result.Fail(senderAccountsResult.Errors);
        }
        
        if (receiverAccountsResult.IsFailed)
        {
            _logger.Error(new {Message = receiverAccountsResult.Errors,Method = nameof(TransferMoneyAsync), TransferDetails = transferDto.ToJson()}.ToJson());

            return Result.Fail(receiverAccountsResult.Errors);
        }

        var participantIds = GetConditionalIds(senderAccountsResult.Value,receiverAccountsResult.Value,transferDto,conditions);
        var conditionalErrorResult = ConditionErrorHandling(participantIds, conditions);
        if (conditionalErrorResult.IsFailed)
        {
            _logger.Error(new {Message = conditionalErrorResult.Errors,Method = nameof(TransferMoneyAsync), TransferDetails = transferDto.ToJson()}.ToJson());

            return Result.Fail(conditionalErrorResult.Errors);
        }
        
        var transactionResult = await _transactionManager.CreateTransactionAsync(participantIds,
            transferDto.Amount, transferDto.Fee, 2);
        if (transactionResult.IsFailed)
        {
            _logger.Error(new {Message = transactionResult.Errors,Method = nameof(TransferMoneyAsync), TransferDetails = transferDto.ToJson()}.ToJson());

            return Result.Fail(transactionResult.Errors);
        }

        var transferResult = await _accountManager.TransferMoneyAsync(transactionResult.Value);
        if (transferResult.IsFailed)
        {
            _logger.Error(new {Message = transferResult.Errors,Method = nameof(TransferMoneyAsync), TransferDetails = transferDto.ToJson()}.ToJson());

            return Result.Fail(transferResult.Errors);
        }
        _logger.Info(new {Message = "Transfer successfully completed.",Method = nameof(TransferMoneyAsync), TransferDetails = transferDto.ToJson()}.ToJson());
        
        return Result.Ok();
    }

    //The reason why this method is not in the Manager class is that establishing
    //mutual reference relationships between classes is not possible,
    //and an object that does not belong to that class is utilized.
    private TransferParticipants GetConditionalIds(List<CustomerAccountModel> senderAccounts,List<CustomerAccountModel> receiverAccounts, TransferDto transferDto, TransferConditions conditions)
    {
        var participants = new TransferParticipants();
        switch (conditions)
        {
            //From customer's main account to receiver's main account
            case TransferConditions.CToC:
                participants.SenderId = senderAccounts.First().CustomerId;
                participants.SenderAccountId = senderAccounts.FirstOrDefault(a => a.IsMain).AccountId;
                participants.ReceiverId = receiverAccounts.First().CustomerId;
                participants.ReceiverAccountId = receiverAccounts.FirstOrDefault(a => a.IsMain).AccountId;
                break;
            //From customer's certain account to receiver's certain account
            case TransferConditions.AToA:
                participants.SenderId = senderAccounts.First().CustomerId;
                participants.SenderAccountId = transferDto.SenderId;
                participants.ReceiverId = receiverAccounts.First().CustomerId;
                participants.ReceiverAccountId = transferDto.ReceiverId;
                break;
            //From customer's main account to receiver's certain account
            case TransferConditions.CToA:
                participants.SenderId = senderAccounts.First().CustomerId;
                participants.SenderAccountId = senderAccounts.FirstOrDefault(a => a.IsMain).AccountId;
                participants.ReceiverId = receiverAccounts.First().CustomerId;
                participants.ReceiverAccountId = transferDto.ReceiverId;
                break;
            //From customer's certain account to receiver's main account
            case TransferConditions.AToC:
                participants.SenderId = senderAccounts.First().CustomerId;
                participants.SenderAccountId = transferDto.SenderId;
                participants.ReceiverAccountId = receiverAccounts.FirstOrDefault(a => a.IsMain).AccountId;
                participants.ReceiverId = receiverAccounts.First().CustomerId;
                break;
        }
        _logger.Info(new {Message = "Participants determined.",Method = nameof(GetConditionalIds), ReceiverCustomerId = participants.ReceiverId, SenderCustomerId = participants.SenderId, participants.ReceiverAccountId, participants.SenderAccountId}.ToJson());

        return participants;
    }
    
    private Result ConditionErrorHandling(TransferParticipants participants,TransferConditions conditions)
    {
        if (participants.SenderAccountId == participants.ReceiverAccountId)
        {
            return Result.Fail(ErrorMessages.SenderIsReceiver);
        }
        
        switch (conditions)
        {
            case TransferConditions.CToC:
                if (participants.SenderId == participants.ReceiverId)
                {
                    return Result.Fail(ErrorMessages.SenderIsReceiver);
                }
                break;
        }
        
        return Result.Ok();
    }

    private async Task<(Result<List<CustomerAccountModel>> Sender, Result<List<CustomerAccountModel>> Receiver)> FilterAccountsByCondition(TransferDto transferDto, TransferConditions conditions)
    {
        Task<Result<List<CustomerAccountModel>>> senderAccountsTask;
        Task<Result<List<CustomerAccountModel>>> receiverAccountsTask;

        switch (conditions)
        {
            case TransferConditions.CToC:
                senderAccountsTask = _accountManager.GetAccountsAsync(transferDto.SenderId);
                receiverAccountsTask = _accountManager.GetAccountsAsync(transferDto.ReceiverId);
                break;
            case TransferConditions.AToA:
                senderAccountsTask = _accountManager.GetAccountsByAccountAsync(transferDto.SenderId);
                receiverAccountsTask = _accountManager.GetAccountsByAccountAsync(transferDto.ReceiverId);
                break;
            case TransferConditions.CToA:
                senderAccountsTask = _accountManager.GetAccountsAsync(transferDto.SenderId);
                receiverAccountsTask = _accountManager.GetAccountsByAccountAsync(transferDto.ReceiverId);
                break;
            case TransferConditions.AToC:
                senderAccountsTask = _accountManager.GetAccountsByAccountAsync(transferDto.SenderId);
                receiverAccountsTask = _accountManager.GetAccountsAsync(transferDto.ReceiverId);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(conditions), conditions, null);
        }

        await Task.WhenAll(senderAccountsTask, receiverAccountsTask);

        return (senderAccountsTask.Result, receiverAccountsTask.Result);
    }
}