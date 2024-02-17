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
        Task<Result<List<CustomerAccountModel>>> senderAccountsResult;
        Task<Result<List<CustomerAccountModel>>> receiverAccountsResult;
        
        FilterAccountsByCondition(out senderAccountsResult,out receiverAccountsResult,transferDto,conditions);
        
        senderAccountsResult = _accountManager.GetAccountsAsync(transferDto.SenderId);
        
        receiverAccountsResult = _accountManager.GetAccountsAsync(transferDto.ReceiverId);

        await Task.WhenAll(senderAccountsResult, receiverAccountsResult);
        
        if (senderAccountsResult.Result.IsFailed)
        {
            return Result.Fail(senderAccountsResult.Result.Errors);
        }
        
        if (receiverAccountsResult.Result.IsFailed)
        {
            return Result.Fail(receiverAccountsResult.Result.Errors);
        }

        var participantIds = GetConditionalIds(senderAccountsResult.Result.Value,receiverAccountsResult.Result.Value,transferDto,conditions);
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

    private void FilterAccountsByCondition(out Task<Result<List<CustomerAccountModel>>> sender,
        out Task<Result<List<CustomerAccountModel>>> receiver, TransferDto transferDto, TransferConditions conditions)
    {
        switch (conditions)
        {
            case TransferConditions.CToC:
                sender = _accountManager.GetAccountsAsync(transferDto.SenderId);
                receiver = _accountManager.GetAccountsAsync(transferDto.ReceiverId);
                break;
            case TransferConditions.AToA:
                sender = _accountManager.GetAccountsByAccountAsync(transferDto.SenderId);
                receiver = _accountManager.GetAccountsByAccountAsync(transferDto.ReceiverId);
                break;
            case TransferConditions.CToA:
                sender = _accountManager.GetAccountsAsync(transferDto.SenderId);
                receiver = _accountManager.GetAccountsByAccountAsync(transferDto.ReceiverId);
                break;
            case TransferConditions.AToC:
                sender = _accountManager.GetAccountsByAccountAsync(transferDto.SenderId);
                receiver = _accountManager.GetAccountsAsync(transferDto.ReceiverId);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(conditions), conditions, null);
        }
    }
}