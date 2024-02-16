using TransactionFlow.Core.DataAccess;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.DataAccess.Abstraction;

public interface ICustomerDal:IEntityRepository<Customer>
{
    List<CustomerAccount> GetAccounts(Customer customer);
    Task<List<CustomerAccount>> GetAccountsAsync(Customer customer);
    Task<List<CustomerAccount>> GetAccountsAsync(int customerId);
    Customer GetCustomerWithAccounts(int customerId);
}