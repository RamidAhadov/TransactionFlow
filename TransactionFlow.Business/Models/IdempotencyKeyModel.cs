using TransactionFlow.Business.Models.Abstraction;

namespace TransactionFlow.Business.Models;

public class IdempotencyKeyModel:IBusinessModel
{
    public string Key { get; set; }
    public DateTime CreateDate { get; set; }
    public string RequestMethod { get; set; }
    public string RequestPath { get; set; }
    public string? RequestParameters { get; set; }
    public int ResponseCode { get; set; }
    public string? ResponseBody { get; set; }
}