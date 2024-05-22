using System.Diagnostics;
using AutoMapper;
using FluentResults;
using Microsoft.Data.SqlClient;
using NLog;
using NuGet.Protocol;
using TransactionFlow.Business.Abstraction;
using TransactionFlow.Business.Models;
using TransactionFlow.Core.Constants;
using TransactionFlow.DataAccess.Abstraction;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.Business.Concrete;

public class IdempotencyManager:IIdempotencyManager
{
    private readonly IIdempotencyDal _idempotencyDal;
    private readonly IMapper _mapper;
    private readonly Logger _logger;
    
    public IdempotencyManager(IIdempotencyDal idempotencyDal, IMapper mapper)
    {
        _idempotencyDal = idempotencyDal;
        _mapper = mapper;
        _logger = LogManager.GetLogger("IdempotencyManagerLogger");
    }


    public Result SetKey(IdempotencyKeyModel key)
    {
        var sw = Stopwatch.StartNew();
        key.CreateDate = DateTime.Now;
        try
        {
            _idempotencyDal.Add(_mapper.Map<IdempotencyKey>(key));
            _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(SetKey), Message = "Idempotency key recorded.",Key = key }.ToJson());

            return Result.Ok();
        }
        catch (SqlException sqlException)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(SetKey), Message = sqlException.InnerException?.Message ?? sqlException.Message, Key = key}.ToJson());
            
            return Result.Fail(ErrorMessages.IdempotencyKeyNotSet);
        }
    }

    public Result<IdempotencyKeyModel> GetValueByKey(long key)
    {
        var sw = Stopwatch.StartNew();
        IdempotencyKeyModel? idempotencyKey;
        try
        {
            idempotencyKey = _mapper.Map<IdempotencyKeyModel>(_idempotencyDal.Get(i => i.Key == key));
        }
        catch (SqlException sqlException)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(GetValueByKey), Message = sqlException.InnerException?.Message ?? sqlException.Message, Key = key}.ToJson());
            
            return Result.Fail(ErrorMessages.IdempotencyKeySearchError);
        }
        
        if (idempotencyKey == null)
        {
            _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(GetValueByKey), Message = "Idempotency key not exists in records.",Key = key }.ToJson());
            
            return Result.Ok();
        }
        _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(GetValueByKey), Message = "Idempotency has found in records.",Key = key }.ToJson());
        
        return Result.Ok(idempotencyKey);
    }

    public Result<long> GenerateNewKey()
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var newKey = _idempotencyDal.GenerateKey();
            _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(GenerateNewKey), Message = "New key generated",Key = newKey }.ToJson());
            
            return newKey;
        }
        catch (SqlException sqlException)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(GenerateNewKey), Message = sqlException.InnerException?.Message ?? sqlException.Message}.ToJson());

            return Result.Fail(ErrorMessages.KeyNotGenerated);
        }
    }
}