namespace TransactionFlow.Entities.Concrete;

public class CustomerArchive:IEntity
{
    public int CustomerId { get; set; }
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string LastName { get; set; }
    public string? Address { get; set; }
    public DateTime RegisterDate { get; set; }
    public DateTime ArchiveDate { get; private set; } = DateTime.Now;
    public int AccountCount { get; set; }
}