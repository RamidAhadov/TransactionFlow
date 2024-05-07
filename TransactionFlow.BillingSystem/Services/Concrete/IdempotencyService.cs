using System.Collections.Concurrent;
using System.Net;
using AutoMapper;
using NuGet.Protocol;
using TransactionFlow.BillingSystem.Models.Dtos;
using TransactionFlow.BillingSystem.Services.Abstraction;
using TransactionFlow.Business.Abstraction;
using TransactionFlow.Business.Models;

namespace TransactionFlow.BillingSystem.Services.Concrete;

public class IdempotencyService:IIdempotencyService
{
    private readonly IIdempotencyManager _idempotencyManager;
    private readonly IMapper _mapper;
    private readonly ConcurrentDictionary<long, object> _locks = new();

    public IdempotencyService(IIdempotencyManager idempotencyManager, IMapper mapper)
    {
        _idempotencyManager = idempotencyManager;
        _mapper = mapper;
    }

    public void Set(HttpRequest request, HttpStatusCode responseCode, object requestBody,
        string? responseBody = default)
    {
        var headerKey = request.Headers["Idempotency-key"].ToString();
        if (long.TryParse(headerKey, out long key))
        {
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
                _ = _idempotencyManager.SetKey(_mapper.Map<IdempotencyKeyModel>(idempotencyKey));
            }
        }
        else
        {
            throw new InvalidCastException();
        }
    }

    public string? Get(string input)
    {
        if (long.TryParse(input, out long key))
        {
            lock (GetLockObject(key))
            {
                var keyResult = _idempotencyManager.GetValueByKey(key);
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

        throw new InvalidCastException();
    }
    public int GenerateKey()
    {
        var keyResult = _idempotencyManager.GenerateNewKey();
        if (keyResult.IsFailed)
        {
            throw new InvalidOperationException();
        }

        return keyResult.Value;
    }

    private object GetLockObject(long key)
    {
        return _locks.GetOrAdd(key, _ => new object());
    }
}