namespace TransactionFlow.Entities.Concrete.Archive;

public class CustomerAccountArchive:IEntity
{
    public int AccountId { get; set; }
    public int CustomerId { get; set; }
    public decimal LastBalance { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ArchiveDate { get; private set; } = DateTime.Now;
    public bool WasMain { get; set; }
}