

namespace TransactionFlow.Entities.Concrete;

public class Transaction:IEntity
{
    public int Id { get; }
    public int CustomerId { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal TransactionAmount { get; set; }
    public short TransactionType { get; set; }
}