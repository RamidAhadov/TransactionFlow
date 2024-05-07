using TransactionFlow.Core.DataAccess.EntityFramework;
using TransactionFlow.DataAccess.Abstraction;
using TransactionFlow.DataAccess.Concrete.EntityFramework.Contexts;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.DataAccess.Concrete.EntityFramework;

public class EfIdempotencyDal:EfEntityRepositoryBase<IdempotencyKey,TransactionContext>,IIdempotencyDal
{
    public long GenerateKey()
    {
        using (var context = new TransactionContext())
        {
            var key = new Key
            {
                GenerateDate = DateTime.Now
            };
            context.GeneratedKeys.Add(key);
            context.SaveChanges();

            return key.Id;
        }
    }
}