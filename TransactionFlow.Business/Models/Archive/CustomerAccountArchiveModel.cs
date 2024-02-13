using TransactionFlow.Business.Models.Abstraction;

namespace TransactionFlow.Business.Models.Archive;

public class CustomerAccountArchiveModel:IBusinessModel
{
    public int AccountId { get; set; }
    public int CustomerId { get; set; }
    public decimal LastBalance { get; set; }
    public DateTime CreatedDate { get; init; }
    public DateTime ArchiveDate { get; private set; } = DateTime.Now;
    public bool WasMain { get; set; }
}