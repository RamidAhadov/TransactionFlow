namespace TransactionFlow.BillingSystem.Services.Abstraction;

public interface ISessionService
{
    void Set(string key, object value);
    object? Get(string key);
}