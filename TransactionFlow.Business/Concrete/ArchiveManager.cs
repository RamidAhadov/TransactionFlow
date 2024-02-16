using AutoMapper;
using FluentResults;
using TransactionFlow.Business.Abstraction;
using TransactionFlow.Business.Models;
using TransactionFlow.Business.Models.Archive;
using TransactionFlow.Core.Constants;
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

    public async Task<Result> ArchiveCustomerAndAccountsAsync(CustomerArchiveModel customerArchiveModel)
    {
        try
        {
            var customerArchive = _mapper.Map<CustomerArchive>(customerArchiveModel);
            var customerAccounts = _mapper.Map<List<CustomerAccountArchive>>(customerArchiveModel.Accounts);
            customerArchive.AccountCount = customerAccounts.Count;
            await _archiveDal.ArchiveAsync(customerArchive,customerAccounts);
            
            return Result.Ok();
        }
        catch (Exception)
        {
            return Result.Fail(ErrorMessages.ArchiveFailed);
        }
    }

    public async Task<Result> ArchiveAccountAsync(CustomerAccountArchiveModel accountArchiveModel)
    {
        try
        {
            await _archiveDal.ArchiveAsync(_mapper.Map<CustomerAccountArchive>(accountArchiveModel));
            
            return Result.Ok();
        }
        catch (Exception)
        {
            return Result.Fail(ErrorMessages.ArchiveFailed);
        }
    }
}