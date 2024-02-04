using TransactionFlow.Business.Abstraction;
using TransactionFlow.Core.Utilities.Results;
using TransactionFlow.DataAccess.Abstraction;
using TransactionFlow.Entities.Concrete;
using TransactionFlow.Entities.Concrete.Dtos;

namespace TransactionFlow.Business.Concrete;

public class CustomerService:ICustomerService
{
    private ICustomerDal _customerDal;

    public CustomerService(ICustomerDal customerDal)
    {
        _customerDal = customerDal;
    }

    public IDataResult<List<Customer>> GetAllCustomers()
    {
        return new SuccessDataResult<List<Customer>>(_customerDal.GetList());
    }

    public IResult Add(Customer customer)
    {
        if (customer == null)
            return new ErrorResult();
        _customerDal.Add(customer);
        return new SuccessResult();
    }

    public Task<IResult> AddAsync(Customer customer)
    {
        throw new NotImplementedException();
    }
}