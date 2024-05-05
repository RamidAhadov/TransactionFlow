namespace TransactionFlow.BillingSystem.Services.Abstraction;

public interface IIdempotencyService
{
    void Set(string key, object? value = default);
    string? Get(string key);
}