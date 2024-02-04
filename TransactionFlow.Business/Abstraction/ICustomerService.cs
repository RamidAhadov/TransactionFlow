using TransactionFlow.Core.Utilities.Results;
using TransactionFlow.Entities.Concrete;
using TransactionFlow.Entities.Concrete.Dtos;

namespace TransactionFlow.Business.Abstraction;

public interface ICustomerService
{
    IDataResult<List<Customer>> GetAllCustomers();
    IResult Add(Customer customer);
    Task<IResult> AddAsync(Customer customer);
}