

namespace TransactionFlow.Entities.Concrete;

public class Transaction:IEntity
{
    public int Id { get; }
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal TransactionAmount { get; set; }
    public decimal? ServiceFee { get; set; }
    public short TransactionType { get; set; }
}