using TransactionFlow.Core.DataAccess;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.DataAccess.Abstraction;

public interface IMemoryDal:IEntityRepository<IdempotencyKey>
{
    
}