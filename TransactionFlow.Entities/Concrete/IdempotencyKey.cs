using System.ComponentModel.DataAnnotations.Schema;

namespace TransactionFlow.Entities.Concrete;

public class IdempotencyKey:IEntity
{ 
    public int Id { get; set; }
    public DateTime CreateDate { get; set; }
    public string Key { get; set; }
    public string RequestMethod { get; set; }
    public string RequestPath { get; set; }
    public string? RequestParameters { get; set; }
    public int ResponseCode { get; set; }
    public string? ResponseBody { get; set; }
}