using Microsoft.EntityFrameworkCore;
using TransactionFlow.Core.Constants;
using TransactionFlow.Core.DataAccess.EntityFramework;
using TransactionFlow.DataAccess.Abstraction;
using TransactionFlow.DataAccess.Concrete.EntityFramework.Contexts;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.DataAccess.Concrete.EntityFramework;

public class EfCustomerAccountDal:EfEntityRepositoryBase<CustomerAccount,TransactionContext>,ICustomerAccountDal
{
    public async Task TransferAsync(Transaction transferDetails)
    {
        int senderId = transferDetails.SenderId;
        int receiverId = transferDetails.ReceiverId;
        
        var amount = transferDetails.TransactionAmount;
        var fee = transferDetails.ServiceFee;
        
        await using (var dbContext = new TransactionContext())
        {
            await using (var transaction = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var sender = await dbContext.CustomerAccounts.FirstOrDefaultAsync(ca => ca.CustomerId == senderId);
                    if (sender == null)
                    {
                        throw new InvalidOperationException(ErrorMessages.AccountNotFound);
                    }
                    if (sender.Balance < amount + fee)
                    {
                        throw new InvalidOperationException(InfoMessages.InsufficientFund);
                    }
                    sender.Balance -= amount + fee;

                    var receiver = await dbContext.CustomerAccounts.FirstOrDefaultAsync(ca => ca.CustomerId == receiverId);
                    if (receiver == null)
                    {
                        throw new InvalidOperationException(ErrorMessages.AccountNotFound);
                    }
                    receiver.Balance += amount;

                    var transactionDetails =
                        await dbContext.Transactions.FirstOrDefaultAsync(t => t.Id == transferDetails.Id);
                    transactionDetails.TransactionStatus = true;

                    await dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch (Exception exception)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(exception.Message);
                }
            }
        }
    }
}