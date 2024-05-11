using System.Collections.Concurrent;
using System.Net;
using AutoMapper;
using FluentResults;
using NuGet.Protocol;
using TransactionFlow.BillingSystem.Models.Dtos;
using TransactionFlow.BillingSystem.Services.Abstraction;
using TransactionFlow.Business.Abstraction;
using TransactionFlow.Business.Models;
using TransactionFlow.Core.Constants;

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

    public Result Set(HttpRequest request, HttpStatusCode responseCode, object requestBody,
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
                var setResult = _idempotencyManager.SetKey(_mapper.Map<IdempotencyKeyModel>(idempotencyKey));
                if (setResult.IsFailed)
                {
                    return Result.Fail(setResult.Errors);
                }
                
                return Result.Ok();
            }
        }
        
        return Result.Fail(ErrorMessages.WrongKeyFormat);
    }

    public Result<string?> Get(string input)
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
                            return Result.Ok(string.Empty);
                        }
                
                        return Result.Ok(idempotencyKey.ResponseBody);
                    }
                }

                return Result.Ok();
            }
        }

        return Result.Fail(ErrorMessages.WrongKeyFormat);
    }
    public Result<long> GenerateKey()
    {
        var keyResult = _idempotencyManager.GenerateNewKey();
        if (keyResult.IsFailed)
        {
            return Result.Fail(keyResult.Errors);
        }

        return Result.Ok(keyResult.Value);
    }

    private object GetLockObject(long key)
    {
        return _locks.GetOrAdd(key, _ => new object());
    }
}