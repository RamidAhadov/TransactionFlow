namespace TransactionFlow.Entities.Concrete;

public class TransactionDetails
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public int SenderAccountId { get; set; }
    public int ReceiverId { get; set; }
    public int ReceiverAccountId { get; set; }
    public decimal TransactionAmount { get; set; }
    public decimal ServiceFee { get; set; }
}