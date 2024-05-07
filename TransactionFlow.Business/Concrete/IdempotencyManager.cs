using AutoMapper;
using FluentResults;
using Microsoft.Data.SqlClient;
using TransactionFlow.Business.Abstraction;
using TransactionFlow.Business.Models;
using TransactionFlow.Core.Constants;
using TransactionFlow.DataAccess.Abstraction;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.Business.Concrete;

public class IdempotencyManager:IIdempotencyManager
{
    private IIdempotencyDal _idempotencyDal;
    private IMapper _mapper;
    
    public IdempotencyManager(IIdempotencyDal idempotencyDal, IMapper mapper)
    {
        _idempotencyDal = idempotencyDal;
        _mapper = mapper;
    }


    public Result SetKey(IdempotencyKeyModel key)
    {
        key.CreateDate = DateTime.Now;
        try
        {
            _idempotencyDal.Add(_mapper.Map<IdempotencyKey>(key));
            
            return Result.Ok();
        }
        catch (SqlException)
        {
            return Result.Fail(ErrorMessages.IdempotencyKeyNotSet);
        }
    }

    public Result<IdempotencyKeyModel> GetValueByKey(long key)
    {
        IdempotencyKeyModel? idempotencyKey;
        try
        {
            idempotencyKey = _mapper.Map<IdempotencyKeyModel>(_idempotencyDal.Get(i => i.Key == key));
        }
        catch (SqlException)
        {
            return Result.Fail(ErrorMessages.IdempotencyKeySearchError);
        }
        
        if (idempotencyKey == null)
        {
            return Result.Ok();
        }

        return Result.Ok(idempotencyKey);
    }

    public Result<int> GenerateNewKey()
    {
        try
        {
            return _idempotencyDal.GenerateKey();
        }
        catch (SqlException)
        {
            return Result.Fail(ErrorMessages.KeyNotGenerated);
        }
    }
}