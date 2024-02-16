using FluentResults;
using TransactionFlow.BillingSystem.Models.Dtos;
using TransactionFlow.Business.Models;

namespace TransactionFlow.BillingSystem.Services.Abstraction;

public interface IAccountService
{
    Task<Result> CreateCustomerAsync(CustomerDto customerDto);
    Task<Result> DeleteCustomerAsync(int customerId);
    Task<Result> CreateAccountAsync(int customerId);
    Task<Result> DeleteAccountAsync(int accountId);
    Task<Result> DeactivateAccountAsync(int accountId);
    Task<Result> ActivateAccountAsync(int accountId);
    Result<CustomerModel> GetCustomer(int customerId);
}