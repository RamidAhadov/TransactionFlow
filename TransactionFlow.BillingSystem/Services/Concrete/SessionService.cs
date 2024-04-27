using TransactionFlow.BillingSystem.Services.Abstraction;

namespace TransactionFlow.BillingSystem.Services.Concrete;

public class SessionService:ISessionService
{
    private static readonly Dictionary<string, object?> cache = new();
    
    public void Set(string key, object? value)
    {
        cache.TryAdd(key, value ?? new object());
    }

    public object? Get(string key)
    {
        return cache.GetValueOrDefault(key);
    }
}