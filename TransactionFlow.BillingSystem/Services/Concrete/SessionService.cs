using TransactionFlow.BillingSystem.Services.Abstraction;

namespace TransactionFlow.BillingSystem.Services.Concrete;

public class SessionService:ISessionService
{
    private Dictionary<string, object?> cache = new();
    
    public void Set(string key, object? value)
    {
        cache.TryAdd(key, value ?? default);
    }

    public object? Get(string key)
    {
        return cache.GetValueOrDefault(key);
    }
}