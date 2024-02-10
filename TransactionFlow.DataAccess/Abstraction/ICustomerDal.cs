using TransactionFlow.Core.DataAccess;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.DataAccess.Abstraction;

public interface ICustomerDal:IEntityRepository<Customer>
{
    List<CustomerAccount> GetAccounts(Customer customer);
}