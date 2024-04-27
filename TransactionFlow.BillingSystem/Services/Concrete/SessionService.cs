using TransactionFlow.BillingSystem.Services.Abstraction;

namespace TransactionFlow.BillingSystem.Services.Concrete;

public class SessionService:ISessionService
{
    private static readonly Dictionary<string, object?> cache = new();
    private Dictionary<string, object> locks = new();
    
    public void Set(string key, object? value)
    {
        lock (GetLockObject(key))
        {
            cache.TryAdd(key, value ?? new object());
        }
    }

    public object? Get(string key)
    {
        return cache.GetValueOrDefault(key);
    }
    
    private object GetLockObject(string key)
    {
        lock (locks)
        {
            if (!locks.ContainsKey(key))
            {
                locks[key] = new object();
            }
            return locks[key];
        }
    }
}