using System.Diagnostics;
using AutoMapper;
using FluentResults;
using NLog;
using NuGet.Protocol;
using TransactionFlow.Business.Abstraction;
using TransactionFlow.Business.Models.Archive;
using TransactionFlow.Core.Constants;
using TransactionFlow.DataAccess.Abstraction;
using TransactionFlow.Entities.Concrete.Archive;

namespace TransactionFlow.Business.Concrete;

public class ArchiveManager:IArchiveManager
{
    private readonly IArchiveDal _archiveDal;
    private readonly IMapper _mapper;
    private readonly Logger _logger;

    public ArchiveManager(IArchiveDal archiveDal, IMapper mapper)
    {
        _archiveDal = archiveDal;
        _mapper = mapper;
        _logger = LogManager.GetLogger("ArchiveManagerLogger");
    }

    public async Task<Result> ArchiveCustomerAndAccountsAsync(CustomerArchiveModel customerArchiveModel)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var customerArchive = _mapper.Map<CustomerArchive>(customerArchiveModel);
            var customerAccounts = _mapper.Map<List<CustomerAccountArchive>>(customerArchiveModel.Accounts);
            customerArchive.AccountCount = customerAccounts.Count;
            await _archiveDal.ArchiveAsync(customerArchive,customerAccounts);
            _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(ArchiveCustomerAndAccountsAsync), Message = "Customer and account(s) archived.", customerArchiveModel.CustomerId }.ToJson());
            
            return Result.Ok();
        }
        catch (Exception exception)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(ArchiveCustomerAndAccountsAsync), Message = exception.InnerException?.Message ?? exception.Message}.ToJson());
            
            return Result.Fail(ErrorMessages.ArchiveFailed);
        }
    }

    public async Task<Result> ArchiveAccountAsync(CustomerAccountArchiveModel accountArchiveModel)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            await _archiveDal.ArchiveAsync(_mapper.Map<CustomerAccountArchive>(accountArchiveModel));
            _logger.Info(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(ArchiveAccountAsync), Message = "Account archived.", accountArchiveModel.AccountId }.ToJson());
            
            return Result.Ok();
        }
        catch (Exception exception)
        {
            _logger.Error(new {Elapsed = $"{sw.ElapsedMilliseconds} ms", Method = nameof(ArchiveAccountAsync), Message = exception.InnerException?.Message ?? exception.Message}.ToJson());
            
            return Result.Fail(ErrorMessages.ArchiveFailed);
        }
    }
}