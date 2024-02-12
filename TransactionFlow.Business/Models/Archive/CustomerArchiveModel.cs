using TransactionFlow.Business.Models.Abstraction;

namespace TransactionFlow.Business.Models.Archive;

public class CustomerArchiveModel:IBusinessModel
{
    public int CustomerId { get; set; }
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string LastName { get; set; }
    public string? Address { get; set; }
    public DateTime RegisterDate { get; init; }
    public DateTime ArchiveDate { get; init; }
    public int AccountCount { get; set; }
    
    public List<CustomerAccountArchiveModel>? Accounts { get; set; }
}