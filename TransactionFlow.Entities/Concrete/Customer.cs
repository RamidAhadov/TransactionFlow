using System.Collections;

namespace TransactionFlow.Entities.Concrete;

public class Customer:IEntity
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string LastName { get; set; }
    public string? Address { get; set; }
    public DateTime RegisterDate { get; set; }

    public virtual ICollection<CustomerAccount> CustomerAccounts { get; set; }
}