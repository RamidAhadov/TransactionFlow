namespace TransactionFlow.Entities.Concrete;

public class Customer:IEntity
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string LastName { get; set; }
    public string? Address { get; set; }
    public DateTime RegisterDate { get; private set; } = DateTime.Now;

    public List<CustomerAccount> CustomerAccounts { get; set; }
}