using TransactionFlow.Business.Models.Abstraction;

namespace TransactionFlow.Business.Models;

public class TransactionModel:IBusinessModel
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public int SenderAccountId { get; set; }
    public int ReceiverId { get; set; }
    public int ReceiverAccountId { get; set; }
    public DateTime TransactionDate { get; init; } = DateTime.Now;
    public decimal TransactionAmount { get; set; }
    public decimal ServiceFee { get; set; }
    public short TransactionType { get; set; }
    public bool TransactionStatus { get; set; }
}