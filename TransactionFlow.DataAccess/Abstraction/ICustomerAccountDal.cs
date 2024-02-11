using TransactionFlow.Core.DataAccess;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.DataAccess.Abstraction;

public interface ICustomerAccountDal:IEntityRepository<CustomerAccount>
{
    Task TransferAsync(Transaction transferDetails);
    Task ChangeMainAccount(int customerId);
    Task TransferToMainAsync(int accountId);
}