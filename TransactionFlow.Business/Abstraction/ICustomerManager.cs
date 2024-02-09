using FluentResults;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.Business.Abstraction;

public interface ICustomerManager
{
    Result<List<Customer>> GetAllCustomers();
    Result Add(Customer customer);
    Result Update(Customer customer);
    Result Delete(int id);
    Task<Result<Customer>> AddAsync(Customer customer);
    Task<Result<Customer>> DeleteCustomerAsync(int customerId);
    Result<Customer> GetCustomerById(int id);
}