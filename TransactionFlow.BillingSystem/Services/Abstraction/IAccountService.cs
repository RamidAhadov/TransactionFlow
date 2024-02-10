using FluentResults;
using TransactionFlow.BillingSystem.Models.Dtos;

namespace TransactionFlow.BillingSystem.Services.Abstraction;

public interface IAccountService
{
    Task<Result> CreateCustomerAsync(CustomerDto customerDto);
    Task<Result> CreateAccountAsync(int customerId);
}