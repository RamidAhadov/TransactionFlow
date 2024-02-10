using TransactionFlow.BillingSystem.Models.Dtos.Abstraction;

namespace TransactionFlow.BillingSystem.Models.Dtos;

public class CustomerDto:IDto
{
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string LastName { get; set; }
    public string? Address { get; set; }
}