namespace TransactionFlow.BillingSystem.Services.Abstraction;

public interface ISessionService
{
    void Set(string key, object? value = default);
    object? Get(string key);
}