namespace TransactionFlow.Entities.Concrete;

public class Key:IEntity
{
    public long Id { get; set; }
    public DateTime GenerateDate { get; set; }
}