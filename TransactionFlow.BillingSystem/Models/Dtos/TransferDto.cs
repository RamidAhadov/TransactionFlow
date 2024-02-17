using TransactionFlow.BillingSystem.Models.Dtos.Abstraction;

namespace TransactionFlow.BillingSystem.Models.Dtos;

public class TransferDto:IDto
{
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
    public decimal Amount { get; set; }
    public decimal Fee { get; set; }
}