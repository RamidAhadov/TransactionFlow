using TransactionFlow.Core.DataAccess;
using TransactionFlow.Entities.Concrete;
using TransactionFlow.Entities.Concrete.Archive;

namespace TransactionFlow.DataAccess.Abstraction;

public interface IArchiveDal:IEntityRepository<CustomerArchive>
{
    Task ArchiveAsync(CustomerArchive customer,List<CustomerAccountArchive> customerAccount);
}