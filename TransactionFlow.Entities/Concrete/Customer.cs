namespace TransactionFlow.Entities.Concrete;

public class Customer:IEntity
{
    public int Id { get; }
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string LastName { get; set; }
    public string? Address { get; set; }
    public DateTime RegisterDate { get; init; } = DateTime.Now;

    public List<Transaction>? Transactions { get; }
}