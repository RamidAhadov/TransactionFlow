using Microsoft.Extensions.Configuration;
using TransactionFlow.Business.Models.Abstraction;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.Business.Models;

public class CustomerModel:IBusinessModel
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string LastName { get; set; }
    public string? Address { get; set; }
    public DateTime RegisterDate { get; set; }
    public int MaxAllowedAccounts { get; set; }

    public List<CustomerAccountModel>? Accounts { get; set; }
}