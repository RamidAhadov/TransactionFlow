using System.ComponentModel.DataAnnotations.Schema;

namespace TransactionFlow.Entities.Concrete;

public class IdempotencyKey:IEntity
{ 
    public int Id { get; set; }
    public string Key { get; set; }
    public string? Value { get; set; }
    public DateTime CreateDate { get; set; }
}