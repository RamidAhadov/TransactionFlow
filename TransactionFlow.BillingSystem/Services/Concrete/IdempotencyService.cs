using System.Collections.Concurrent;
using System.Net;
using System.Text;
using AutoMapper;
using NuGet.Protocol;
using TransactionFlow.BillingSystem.Models.Dtos;
using TransactionFlow.BillingSystem.Services.Abstraction;
using TransactionFlow.Business.Abstraction;
using TransactionFlow.Business.Models;

namespace TransactionFlow.BillingSystem.Services.Concrete;

public class IdempotencyService:IIdempotencyService
{
    private IMemoryManager _memoryManager;
    private IMapper _mapper;
    private readonly ConcurrentDictionary<string, object> _locks = new();

    public IdempotencyService(IMemoryManager memoryManager, IMapper mapper)
    {
        _memoryManager = memoryManager;
        _mapper = mapper;
    }

    public void Set(HttpRequest request, HttpStatusCode responseCode, object requestBody,
        string? responseBody = default)
    {
        var key = request.Headers["Idempotency-key"].ToString();
        
        if (!request.Body.CanSeek)
        {
            request.EnableBuffering();
        }
        
        var idempotencyKey = new IdempotencyKeyDto
        {
            Key = key,
            RequestMethod = request.Method,
            RequestPath = request.Path,
            RequestParameters = requestBody.ToJson(),
            ResponseCode = (int)responseCode,
            ResponseBody = responseBody
        };
        
        lock (GetLockObject(key))
        {
            _ = _memoryManager.SetKey(_mapper.Map<IdempotencyKeyModel>(idempotencyKey));
        }
    }

    public string? Get(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentNullException();
        }
        
        lock (GetLockObject(key))
        {
            var keyResult = _memoryManager.GetValueByKey(key);
            if (keyResult.IsSuccess)
            {
                var idempotencyKey = _mapper.Map<IdempotencyKeyDto>(keyResult.Value);
                if (idempotencyKey != null)
                {
                    if (idempotencyKey.ResponseBody == null)
                    {
                        return string.Empty;
                    }
                
                    return idempotencyKey.ResponseBody;
                }
            }

            return null;
        }
    }
    public string GenerateKey()
    {
        throw new NotImplementedException();
    }

    public string GenerateKey(params string[] parameters)
    {
        throw new NotImplementedException();
    }

    private object GetLockObject(string key)
    {
        return _locks.GetOrAdd(key, _ => new object());
    }
}