using TransactionFlow.Core.DataAccess;
using TransactionFlow.Entities.Concrete;
using TransactionFlow.Entities.Concrete.Archive;

namespace TransactionFlow.DataAccess.Abstraction;

public interface ICustomerAccountArchiveDal:IEntityRepository<CustomerAccountArchive>
{
    
}