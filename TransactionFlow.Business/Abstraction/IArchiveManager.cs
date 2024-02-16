using FluentResults;
using TransactionFlow.Business.Models;
using TransactionFlow.Business.Models.Archive;
using TransactionFlow.Entities.Concrete.Archive;

namespace TransactionFlow.Business.Abstraction;

public interface IArchiveManager
{
    Task<Result> ArchiveCustomerAndAccountsAsync(CustomerArchiveModel customerArchiveModel);
    Task<Result> ArchiveAccountAsync(CustomerAccountArchiveModel accountArchiveModel);
}