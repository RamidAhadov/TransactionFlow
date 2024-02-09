using TransactionFlow.Core.Utilities.Results;
using TransactionFlow.Entities.Concrete;
using TransactionFlow.Entities.Concrete.Dtos;

namespace TransactionFlow.Business.Abstraction;

public interface ICustomerService
{
    IDataResult<List<Customer>> GetAllCustomers();
    IResult Add(Customer customer);
    IResult Update(Customer customer);
    IResult Delete(int id);
    Task<IDataResult<Customer>> AddAsync(Customer customer);
    Task<IDataResult<Customer>> DeleteCustomerAsync(Customer customer);
    Task<IDataResult<Customer>> DeleteCustomerAsync(int customerId);
    IDataResult<Customer> GetCustomerById(int id);
}