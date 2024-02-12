using TransactionFlow.Core.DataAccess.EntityFramework;
using TransactionFlow.DataAccess.Abstraction;
using TransactionFlow.DataAccess.Concrete.EntityFramework.Contexts;
using TransactionFlow.Entities.Concrete;
using TransactionFlow.Entities.Concrete.Archive;

namespace TransactionFlow.DataAccess.Concrete.EntityFramework;

public class EfCustomerAccountArchiveDal:EfEntityRepositoryBase<CustomerAccountArchive,TransactionContext>,ICustomerAccountArchiveDal
{
    
}