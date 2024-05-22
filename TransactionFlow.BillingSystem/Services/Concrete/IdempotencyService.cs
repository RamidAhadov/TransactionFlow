using System.Collections.Concurrent;
using System.Net;
using AutoMapper;
using FluentResults;
using NLog;
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
    private readonly Logger _logger;

    public IdempotencyService(IIdempotencyManager idempotencyManager, IMapper mapper)
    {
        _idempotencyManager = idempotencyManager;
        _mapper = mapper;
        _logger = LogManager.GetLogger("IdempotencyServiceLogger");
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
            _logger.Info(new { Message = "Key created.", Method = nameof(Set), Key = idempotencyKey.ToJson() }.ToJson());
        
            lock (GetLockObject(key))
            {
                var setResult = _idempotencyManager.SetKey(_mapper.Map<IdempotencyKeyModel>(idempotencyKey));
                if (setResult.IsFailed)
                {
                    _logger.Error(new { Message = setResult.Errors, Method = nameof(Set),Key = idempotencyKey.ToJson()}.ToJson());
                    
                    return Result.Fail(setResult.Errors);
                }
                _logger.Info(new { Message = "Key set.", Method = nameof(Set), Key = idempotencyKey.ToJson()}.ToJson());
                
                return Result.Ok();
            }
        }
        _logger.Error(new { Message = "Key format is not correct.", Method = nameof(Set),Key = headerKey}.ToJson());

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
                        _logger.Info(new { Message = "Key already exists.", Method = nameof(Get), Key = input}.ToJson());
                        if (idempotencyKey.ResponseBody == null)
                        {
                            return Result.Ok(string.Empty);
                        }
                
                        return Result.Ok(idempotencyKey.ResponseBody);
                    }
                }
                _logger.Info(new { Message = "Key was not exist in the base.", Method = nameof(Get), Key = input}.ToJson());

                return Result.Ok();
            }
        }
        _logger.Error(new { Message = "Key format is not correct.", Method = nameof(Set),Key = input}.ToJson());

        return Result.Fail(ErrorMessages.WrongKeyFormat);
    }
    public Result<long> GenerateKey()
    {
        var keyResult = _idempotencyManager.GenerateNewKey();
        if (keyResult.IsFailed)
        {
            _logger.Error(new { Message = keyResult.Errors, Method = nameof(GenerateKey)}.ToJson());
            
            return Result.Fail(keyResult.Errors);
        }
        _logger.Info(new { Message = "Key created.", Method = nameof(GenerateKey), Key = keyResult.Value}.ToJson());

        return Result.Ok(keyResult.Value);
    }

    private object GetLockObject(long key)
    {
        return _locks.GetOrAdd(key, _ => new object());
    }
}