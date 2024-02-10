using FluentResults;
using TransactionFlow.Business.Models;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.Business.Abstraction;

public interface ICustomerManager
{
    Result<List<CustomerModel>> GetAllCustomers();
    Result Create(CustomerModel customer);
    Result Update(CustomerModel customer);
    Result Delete(int id);
    Task<Result<CustomerModel>> CreateAsync(CustomerModel customer);
    Task<Result<CustomerModel>> DeleteCustomerAsync(int customerId);
    Result<CustomerModel> GetCustomerById(int id);
}