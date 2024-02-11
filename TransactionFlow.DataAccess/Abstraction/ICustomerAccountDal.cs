using TransactionFlow.Core.DataAccess;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.DataAccess.Abstraction;

public interface ICustomerAccountDal:IEntityRepository<CustomerAccount>
{
    Task TransferAsync(Transaction transferDetails);
    Task ChangeMainAccountAsync(int customerId);
    Task<CustomerAccount> TransferToMainAsync(int accountId);
}