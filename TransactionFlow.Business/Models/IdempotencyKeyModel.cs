using TransactionFlow.Business.Models.Abstraction;

namespace TransactionFlow.Business.Models;

public class IdempotencyKeyModel:IBusinessModel
{
    public string Key { get; set; }
    public string Value { get; set; }
    public DateTime CreateDate { get; set; }
}