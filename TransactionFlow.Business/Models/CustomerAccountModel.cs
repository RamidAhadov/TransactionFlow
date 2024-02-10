using TransactionFlow.Business.Models.Abstraction;

namespace TransactionFlow.Business.Models;

public class CustomerAccountModel:IBusinessModel
{
    public int AccountId { get; }
    public int CustomerId { get; set; }
    public decimal Balance { get; set; }
    public DateTime CreatedDate { get; }
    public DateTime? LastUpdated { get; set; }
    public bool IsActive { get; set; }
    public bool IsMain { get; set; }
}