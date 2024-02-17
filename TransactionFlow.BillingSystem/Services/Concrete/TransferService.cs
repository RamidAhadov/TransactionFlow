using FluentResults;
using TransactionFlow.BillingSystem.Models.Dtos;
using TransactionFlow.BillingSystem.Services.Abstraction;
using TransactionFlow.Business.Abstraction;
using TransactionFlow.Business.Models;
using TransactionFlow.Core.Constants;
#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace TransactionFlow.BillingSystem.Services.Concrete;

public class TransferService:ITransferService
{
    private IAccountManager _accountManager;
    private ITransactionManager _transactionManager;

    public TransferService(IAccountManager accountManager, ITransactionManager transactionManager)
    {
        _accountManager = accountManager;
        _transactionManager = transactionManager;
    }

    public async Task<Result> TransferMoneyAsync(TransferDto transferDto,TransferConditions conditions)
    {
        Result<List<CustomerAccountModel>>? senderAccountsResult;
        Result<List<CustomerAccountModel>>? receiverAccountsResult;
        
        (senderAccountsResult,receiverAccountsResult) = await FilterAccountsByCondition(transferDto,conditions);
        
        //senderAccountsResult = _accountManager.GetAccountsAsync(transferDto.SenderId);
        
        //receiverAccountsResult = _accountManager.GetAccountsAsync(transferDto.ReceiverId);

        //await Task.WhenAll( senderAccountsResult, receiverAccountsResult);
        
        if (senderAccountsResult.IsFailed)
        {
            return Result.Fail(senderAccountsResult.Errors);
        }
        
        if (receiverAccountsResult.IsFailed)
        {
            return Result.Fail(receiverAccountsResult.Errors);
        }

        var participantIds = GetConditionalIds(senderAccountsResult.Value,receiverAccountsResult.Value,transferDto,conditions);
        var conditionalErrorResult = ConditionErrorHandling(participantIds, conditions);
        if (conditionalErrorResult.IsFailed)
        {
            return Result.Fail(conditionalErrorResult.Errors);
        }
        
        var transactionResult = await _transactionManager.CreateTransactionAsync(participantIds,
            transferDto.Amount, transferDto.Fee, 2);
        if (transactionResult.IsFailed)
        {
            return Result.Fail(transactionResult.Errors);
        }

        var transferResult = await _accountManager.TransferMoneyAsync(transactionResult.Value);
        if (transferResult.IsFailed)
        {
            return Result.Fail(transferResult.Errors);
        }
        
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

        return ( senderAccountsTask.Result, receiverAccountsTask.Result);
    }


    
    private async Task FilterAccountsByCondition1( Result<List<CustomerAccountModel>> sender,
         Result<List<CustomerAccountModel>> receiver,
        TransferDto transferDto,
        TransferConditions conditions)
    {
        switch (conditions)
        {
            case TransferConditions.CToC:
                sender = await _accountManager.GetAccountsAsync(transferDto.SenderId);
                receiver = await _accountManager.GetAccountsAsync(transferDto.ReceiverId);
                break;
            case TransferConditions.AToA:
                sender = await _accountManager.GetAccountsByAccountAsync(transferDto.SenderId);
                receiver = await _accountManager.GetAccountsByAccountAsync(transferDto.ReceiverId);
                break;
            case TransferConditions.CToA:
                sender = await _accountManager.GetAccountsAsync(transferDto.SenderId);
                receiver = await _accountManager.GetAccountsByAccountAsync(transferDto.ReceiverId);
                break;
            case TransferConditions.AToC:
                sender = await _accountManager.GetAccountsByAccountAsync(transferDto.SenderId);
                receiver = await _accountManager.GetAccountsAsync(transferDto.ReceiverId);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(conditions), conditions, null);
        }
    }

}