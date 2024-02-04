using System.ComponentModel;
using TransactionFlow.Business.Abstraction;
using TransactionFlow.Business.Constants;
using TransactionFlow.Core.Utilities.Results;
using TransactionFlow.DataAccess.Abstraction;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.Business.Concrete;

public class TransactionService:ITransactionService
{
    private ITransactionDal _transactionDal;

    public TransactionService(ITransactionDal transactionDal)
    {
        _transactionDal = transactionDal;
    }

    [Description(description:"Count describes last *count transaction(s)")]
    public IDataResult<List<Transaction>> GetTransactions(Customer customer,int? count)
    {
        //_log.Info("This is a log message.");   
        if (customer == null)
            return new ErrorDataResult<List<Transaction>>(ErrorMessages.NullObjectEntered);
        var list = _transactionDal.GetTransactions(customer,count);
        if (list == null)
            return new ErrorDataResult<List<Transaction>>(ErrorMessages.ObjectNotFound);
        if (list.Count == 0)
            return new ErrorDataResult<List<Transaction>>(InfoMessages.ZeroTransactionFound);
        return new SuccessDataResult<List<Transaction>>(list);
    }

    public IDataResult<List<Transaction>> GetSentTransactions(Customer customer, int? count)
    {
        if (customer == null)
            return new ErrorDataResult<List<Transaction>>(ErrorMessages.NullObjectEntered);
        var list = _transactionDal.GetSentTransactions(customer,count);
        if (list == null)
            return new ErrorDataResult<List<Transaction>>(ErrorMessages.ObjectNotFound);
        if (list.Count == 0)
            return new ErrorDataResult<List<Transaction>>(InfoMessages.ZeroTransactionFound);
        return new SuccessDataResult<List<Transaction>>(list);
    }

    public IDataResult<List<Transaction>> GetReceivedTransactions(Customer customer, int? count)
    {
        if (customer == null)
            return new ErrorDataResult<List<Transaction>>(ErrorMessages.NullObjectEntered);
        var list = _transactionDal.GetReceivedTransactions(customer,count);
        if (list == null)
            return new ErrorDataResult<List<Transaction>>(ErrorMessages.ObjectNotFound);
        if (list.Count == 0)
            return new ErrorDataResult<List<Transaction>>(InfoMessages.ZeroTransactionFound);
        return new SuccessDataResult<List<Transaction>>(list);
    }
}