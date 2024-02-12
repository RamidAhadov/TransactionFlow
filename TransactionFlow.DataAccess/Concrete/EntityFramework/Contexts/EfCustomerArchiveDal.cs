using TransactionFlow.Core.DataAccess.EntityFramework;
using TransactionFlow.DataAccess.Abstraction;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.DataAccess.Concrete.EntityFramework.Contexts;

public class EfCustomerArchiveDal:EfEntityRepositoryBase<CustomerArchive,TransactionContext>,ICustomerArchiveDal
{
    
}