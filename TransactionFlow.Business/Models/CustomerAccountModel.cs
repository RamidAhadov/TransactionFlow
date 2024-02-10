using TransactionFlow.Business.Models.Abstraction;

namespace TransactionFlow.Business.Models;

public class CustomerAccountModel:IBusinessModel
{
    public int CustomerId { get; set; }
    public decimal Balance { get;}
    public bool IsActive { get; set; }
    public bool IsMain { get; set; }
}