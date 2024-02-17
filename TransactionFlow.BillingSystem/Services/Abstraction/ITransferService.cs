using FluentResults;
using TransactionFlow.BillingSystem.Models.Dtos;
using TransactionFlow.Business.Models;
using TransactionFlow.Core.Constants;

namespace TransactionFlow.BillingSystem.Services.Abstraction;

public interface ITransferService
{
    Task<Result> TransferMoneyAsync(TransferDto transferDto, TransferConditions condition);
}