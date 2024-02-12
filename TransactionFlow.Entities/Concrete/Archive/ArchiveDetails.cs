namespace TransactionFlow.Entities.Concrete.Archive;

public class ArchiveDetails
{
    public int CustomerId { get; set; }
    public List<int> AccountIds { get; set; }
}