namespace TransactionFlow.Entities.Concrete.Dtos;

public class CustomerDto:IDto
{
    public int Id { get; }
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string LastName { get; set; }
    public string? Address { get; set; }
}