using TransactionFlow.Business.Models.Abstraction;

namespace TransactionFlow.Business.Models;

public class CustomerAccountModel:IBusinessModel
{
    public int AccountId { get; init; }
    public int CustomerId { get; init; }
    public decimal Balance { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsMain { get; set; }
}