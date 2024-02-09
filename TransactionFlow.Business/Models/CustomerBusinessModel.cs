using TransactionFlow.Business.Models.Abstraction;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.Business.Models;

public class CustomerBusinessModel:IBusinessModel
{
    public int Id { get; }
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string LastName { get; set; }
    public string? Address { get; set; }
    public DateTime RegisterDate { get; }
    
    List<CustomerAccount>? Accounts { get; set; }
}