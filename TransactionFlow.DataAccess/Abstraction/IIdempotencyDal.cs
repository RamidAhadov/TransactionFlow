using TransactionFlow.Core.DataAccess;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.DataAccess.Abstraction;

public interface IIdempotencyDal:IEntityRepository<IdempotencyKey>
{
    long GenerateKey();
    long GetLastKey();
}