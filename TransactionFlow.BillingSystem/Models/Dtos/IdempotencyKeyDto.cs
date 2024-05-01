using TransactionFlow.BillingSystem.Models.Dtos.Abstraction;

namespace TransactionFlow.BillingSystem.Models.Dtos;

public class IdempotencyKeyDto:IDto
{
    public string Key { get; set; }
    public string Value { get; set; }
}