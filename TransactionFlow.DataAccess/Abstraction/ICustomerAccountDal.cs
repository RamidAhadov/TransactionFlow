using TransactionFlow.Core.DataAccess;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.DataAccess.Abstraction;

public interface ICustomerAccountDal:IEntityRepository<CustomerAccount>
{
    Task TransferAsync1(Transaction transferDetails);
    Task ChangeMainAccountAsync(int customerId);
    Task<CustomerAccount> TransferAsync(TransactionDetails transactionDetails);
}