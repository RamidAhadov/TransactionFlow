using Microsoft.EntityFrameworkCore;
using TransactionFlow.Core.Constants;
using TransactionFlow.Core.DataAccess.EntityFramework;
using TransactionFlow.DataAccess.Abstraction;
using TransactionFlow.DataAccess.Concrete.EntityFramework.Contexts;
using TransactionFlow.Entities.Concrete;
using InvalidOperationException = System.InvalidOperationException;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace TransactionFlow.DataAccess.Concrete.EntityFramework;

public class EfCustomerAccountDal:EfEntityRepositoryBase<CustomerAccount,TransactionContext>,ICustomerAccountDal
{
    public async Task TransferAsync1(Transaction transferDetails)
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

                    var receiver = await dbContext.CustomerAccounts.FirstOrDefaultAsync(ca => ca.CustomerId == receiverId);
                    if (receiver == null)
                    {
                        throw new InvalidOperationException(ErrorMessages.AccountNotFound);
                    }

                    TransferAmount(sender, receiver, amount, fee);

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

    public async Task ChangeMainAccountAsync(int customerId)
    {
        await using (var dbContext = new TransactionContext())
        {
            await using (var transaction = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var accounts = await dbContext.CustomerAccounts.Where(ca => ca.CustomerId == customerId).ToListAsync();
                    var mainAccount = accounts.SingleOrDefault(a => a.IsMain);
                    var nonMainAccounts = accounts.Where(a => !a.IsMain && a.IsActive).ToList();

                    if (!nonMainAccounts.Any())
                    {
                        throw new InvalidOperationException(ErrorMessages.CustomerHasNotAnotherAccount);
                    }

                    mainAccount.IsMain = false;
                    var nextMainAccount = nonMainAccounts.First(m => m.AccountId != mainAccount.AccountId);
                    nextMainAccount.IsMain = true;

                    await dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch(InvalidOperationException exception)
                {
                    await transaction.RollbackAsync();
                    throw new InvalidOperationException(exception.Message);
                }
                catch (Exception exception)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(exception.Message);
                }
            }
        }
    }

    public async Task<CustomerAccount> TransferAsync(TransactionDetails transactionDetails)
    {
        await using (var dbContext = new TransactionContext())
        {
            await using (var transaction = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var senderAccount = await dbContext.CustomerAccounts.SingleOrDefaultAsync(ca=>ca.AccountId == transactionDetails.SenderId);
                    var receiverAccount =
                        await dbContext.CustomerAccounts.SingleOrDefaultAsync(ca =>
                            ca.AccountId == transactionDetails.ReceiverId);
                    
                    TransferAmount(senderAccount,receiverAccount, transactionDetails.TransactionAmount);

                    var transactionRecord =
                        await dbContext.Transactions.FirstOrDefaultAsync(t => t.Id == transactionDetails.Id);
                    transactionRecord.TransactionStatus = true;

                    await dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return senderAccount;
                }
                catch (Exception exception)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(exception.Message);
                }
            }
        }
    }

    private static void TransferAmount(CustomerAccount senderAccount, CustomerAccount receiverAccount, decimal amount)
    {
        if (senderAccount.Balance < amount)
        {
            throw new InvalidOperationException(InfoMessages.InsufficientFund);
        }
        
        receiverAccount.Balance += amount;
        senderAccount.Balance = 0;
        senderAccount.LastUpdated = receiverAccount.LastUpdated = DateTime.Now;
    }

    private static void TransferAmount(CustomerAccount senderAccount, CustomerAccount receiverAccount, decimal amount, decimal fee)
    {
        if (senderAccount.Balance < amount + fee)
        {
            throw new InvalidOperationException(InfoMessages.InsufficientFund);
        }

        senderAccount.Balance -= amount + fee;
        receiverAccount.Balance += amount;
        senderAccount.LastUpdated = receiverAccount.LastUpdated = DateTime.Now;
    }
}