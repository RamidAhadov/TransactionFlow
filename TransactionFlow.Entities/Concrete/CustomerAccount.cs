namespace TransactionFlow.Entities.Concrete;

public class CustomerAccount:IEntity
{
    public int AccountId { get; }
    public int CustomerId { get; set; }
    public decimal Balance { get; set; }
    public DateTime CreatedDate { get; private set; } = DateTime.Now;
    public DateTime? LastUpdated { get; set; }
    public bool IsActive { get; set; }
    public bool IsMain { get; set; }
}