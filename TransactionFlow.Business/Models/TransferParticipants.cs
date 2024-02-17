using TransactionFlow.Business.Models.Abstraction;

namespace TransactionFlow.Business.Models;

public class TransferParticipants:IBusinessModel
{
    public int SenderId { get; set; }
    public int SenderAccountId { get; set; }
    public int ReceiverId { get; set; }
    public int ReceiverAccountId { get; set; }
}