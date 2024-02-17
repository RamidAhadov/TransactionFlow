using Autofac;
using TransactionFlow.BillingSystem.Services.Abstraction;
using TransactionFlow.BillingSystem.Services.Concrete;
using TransactionFlow.Business.Abstraction;
using TransactionFlow.Business.Concrete;
using TransactionFlow.DataAccess.Abstraction;
using TransactionFlow.DataAccess.Concrete.EntityFramework;

namespace TransactionFlow.BillingSystem.DependencyResolvers.Autofac;

public class AutofacBusinessModule:Module
{
    protected override void Load(ContainerBuilder builder)
    {
        //Managers
        builder.RegisterType<EfCustomerDal>().As<ICustomerDal>();
        builder.RegisterType<CustomerManager>().As<ICustomerManager>();
        
        builder.RegisterType<EfCustomerAccountDal>().As<ICustomerAccountDal>();
        builder.RegisterType<CustomerAccountManager>().As<IAccountManager>();
        
        builder.RegisterType<EfTransactionDal>().As<ITransactionDal>();
        builder.RegisterType<TransactionManager>().As<ITransactionManager>();
        
        builder.RegisterType<EfArchiveDal>().As<IArchiveDal>();
        builder.RegisterType<ArchiveManager>().As<IArchiveManager>();
        
        //Services
        builder.RegisterType<TransferService>().As<ITransferService>();
        builder.RegisterType<AccountService>().As<IAccountService>();
        builder.RegisterType<TransactionService>().As<ITransactionService>();
    }
}