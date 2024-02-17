namespace TransactionFlow.Entities.Concrete;

public class Transaction:IEntity
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public int SenderAccountId { get; set; }
    public int ReceiverId { get; set; }
    public int ReceiverAccountId { get; set; }
    
    //Try init
    public DateTime TransactionDate { get; private set; } = DateTime.Now;
    public decimal TransactionAmount { get; set; }
    public decimal ServiceFee { get; set; }
    public short TransactionType { get; set; }
    public bool TransactionStatus { get; set; }
}