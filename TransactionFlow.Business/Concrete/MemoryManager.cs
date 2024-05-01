using AutoMapper;
using FluentResults;
using Microsoft.Data.SqlClient;
using TransactionFlow.Business.Abstraction;
using TransactionFlow.Business.Models;
using TransactionFlow.Core.Constants;
using TransactionFlow.DataAccess.Abstraction;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.Business.Concrete;

public class MemoryManager:IMemoryManager
{
    private IMemoryDal _memoryDal;
    private IMapper _mapper;
    
    public MemoryManager(IMemoryDal memoryDal, IMapper mapper)
    {
        _memoryDal = memoryDal;
        _mapper = mapper;
    }


    public Result SetKey(string key)
    {
        var idempotencyKey = new IdempotencyKeyModel
        {
            Key = key,
            CreateDate = DateTime.Now
        };

        try
        {
            _memoryDal.Add(_mapper.Map<IdempotencyKey>(idempotencyKey));
            
            return Result.Ok();
        }
        catch (SqlException)
        {
            return Result.Fail(ErrorMessages.IdempotencyKeyNotSet);
        }
    }

    public Result<IdempotencyKeyModel> GetValueByKey(string key)
    {
        IdempotencyKeyModel? idempotencyKey;
        try
        {
            idempotencyKey = _mapper.Map<IdempotencyKeyModel>(_memoryDal.Get(i => i.Key == key));
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
}