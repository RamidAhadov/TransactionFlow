using TransactionFlow.Core.DataAccess;
using TransactionFlow.Core.DataAccess.EntityFramework;
using TransactionFlow.DataAccess.Concrete.EntityFramework.Contexts;
using TransactionFlow.Entities;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.DataAccess.Abstraction;

public interface ICustomerDal:IEntityRepository<Customer>
{
    
}