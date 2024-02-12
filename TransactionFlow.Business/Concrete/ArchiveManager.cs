using AutoMapper;
using FluentResults;
using TransactionFlow.Business.Abstraction;
using TransactionFlow.Business.Models.Archive;
using TransactionFlow.DataAccess.Abstraction;
using TransactionFlow.Entities.Concrete.Archive;

namespace TransactionFlow.Business.Concrete;

public class ArchiveManager:IArchiveManager
{
    private IArchiveDal _archiveDal;
    private IMapper _mapper;

    public ArchiveManager(IArchiveDal archiveDal, IMapper mapper)
    {
        _archiveDal = archiveDal;
        _mapper = mapper;
    }

    public async Task<Result> ArchiveCustomerAndAccountsAsync(CustomerArchiveModel customerArchiveModel, List<CustomerAccountArchiveModel> accountArchiveModels)
    {
        try
        {
            var customerArchive = _mapper.Map<CustomerArchive>(customerArchiveModel);
            var customerAccounts = _mapper.Map<List<CustomerAccountArchive>>(accountArchiveModels);
            await _archiveDal.ArchiveAsync(customerArchive,customerAccounts);
            
            return Result.Ok();
        }
        catch (Exception e)
        {
            return Result.Fail(e.Message);
        }
    }
}