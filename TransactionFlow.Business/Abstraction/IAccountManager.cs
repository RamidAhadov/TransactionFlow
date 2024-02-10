using FluentResults;
using TransactionFlow.Business.Models;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.Business.Abstraction;

public interface IAccountManager
{
    Task<Result> CreateAccountAsync(CustomerModel customer);
    Task<Result> DeleteAccountAsync(CustomerModel customer,int accountId);
    
}