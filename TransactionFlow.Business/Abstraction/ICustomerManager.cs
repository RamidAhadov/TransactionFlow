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
    Result Delete(CustomerModel customerModel);
    Task<Result<CustomerModel>> CreateAsync(CustomerModel customerModel);
    Task<Result<CustomerModel>> DeleteCustomerAsync(int customerId);
    Task<Result<CustomerModel>> DeleteCustomerAsync(CustomerModel customerModel);
    Result<CustomerModel> GetCustomerById(int id);
}