using FluentResults;
using TransactionFlow.Business.Models;

namespace TransactionFlow.Business.Abstraction;

public interface IMemoryManager
{
    Result SetKey(string key);
    Result<IdempotencyKeyModel> GetValueByKey(string key);
}