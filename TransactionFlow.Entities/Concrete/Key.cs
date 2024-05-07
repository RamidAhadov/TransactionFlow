namespace TransactionFlow.Entities.Concrete;

public class Key:IEntity
{
    public int Id { get; set; }
    public DateTime GenerateDate { get; set; }
}