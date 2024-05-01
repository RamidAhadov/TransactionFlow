using AutoMapper;
using TransactionFlow.BillingSystem.Models.Dtos;
using TransactionFlow.BillingSystem.Services.Abstraction;
using TransactionFlow.Business.Abstraction;

namespace TransactionFlow.BillingSystem.Services.Concrete;

public class SessionService:ISessionService
{
    private IMemoryManager _memoryManager;
    private IMapper _mapper;
    private Dictionary<string, object> locks = new();

    public SessionService(IMemoryManager memoryManager, IMapper mapper)
    {
        _memoryManager = memoryManager;
        _mapper = mapper;
    }

    public void Set(string key, object? value)
    {
        lock (GetLockObject(key))
        {
            _ = _memoryManager.SetKey(key);
        }
    }

    public string? Get(string key)
    {
        var keyResult = _memoryManager.GetValueByKey(key);
        if (keyResult.IsSuccess)
        {
            var idempotencyKey = _mapper.Map<IdempotencyKeyDto>(keyResult.Value);
            if (idempotencyKey != null)
            {
                if (idempotencyKey.Value == null)
                {
                    return string.Empty;
                }
                
                return idempotencyKey.Value;
            }
        }

        return null;
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