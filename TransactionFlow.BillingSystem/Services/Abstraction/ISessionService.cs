namespace TransactionFlow.BillingSystem.Services.Abstraction;

public interface ISessionService
{
    void Set(string key, object? value = default);
    string? Get(string key);
}