using FluentResults;
using TransactionFlow.Business.Models;

namespace TransactionFlow.Business.Abstraction;

public interface IMemoryManager
{
    Result SetKey(IdempotencyKeyModel key);
    Result<IdempotencyKeyModel> GetValueByKey(string key);
}