using FluentResults;
using TransactionFlow.Business.Models;

namespace TransactionFlow.Business.Abstraction;

public interface IIdempotencyManager
{
    Result SetKey(IdempotencyKeyModel key);
    Result<IdempotencyKeyModel> GetValueByKey(long key);
    Result<long> GenerateNewKey();
    Result<long> GenerateNewKeyV2();
}