using TransactionFlow.BillingSystem.Models.Dtos.Abstraction;

namespace TransactionFlow.BillingSystem.Models.Dtos;

public class IdempotencyKeyDto:IDto
{
    public long Key { get; set; }
    public string RequestMethod { get; set; }
    public string RequestPath { get; set; }
    public string? RequestParameters { get; set; }
    public int ResponseCode { get; set; }
    public string? ResponseBody { get; set; }
}