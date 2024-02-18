using FluentResults;
using TransactionFlow.BillingSystem.Models.Dtos;
using TransactionFlow.Business.Models;

namespace TransactionFlow.BillingSystem.Services.Abstraction;

public interface IAccountService
{
    Result<List<CustomerModel>> GetAllCustomers();
    Task<Result> CreateCustomerAsync(CustomerDto customerDto);
    Result UpdateCustomer(int customerId, CustomerDto customerDto);
    Task<Result> DeleteCustomerAsync(int customerId);
    Result CreateAccount(int customerId);
    Task<Result> DeleteAccountAsync(int accountId);
    Task<Result> DeactivateAccountAsync(int accountId);
    Task<Result> ActivateAccountAsync(int accountId);
    Result<CustomerModel> GetCustomer(int customerId);
}