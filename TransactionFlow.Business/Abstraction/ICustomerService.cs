using TransactionFlow.Core.Utilities.Results;
using TransactionFlow.Entities.Concrete;
using TransactionFlow.Entities.Concrete.Dtos;

namespace TransactionFlow.Business.Abstraction;

public interface ICustomerService
{
    IDataResult<List<Customer>> GetAllCustomers();
    IResult Add(Customer customer);
    IResult Update(Customer customer);
    IResult Delete(Customer customer);
    IResult Delete(int id);
    Task<IResult> AddAsync(Customer customer);
    IDataResult<Customer> GetCustomerById(int id);
    IDataResult<List<Transaction>> GetTransactions(Customer customer, int count);
}