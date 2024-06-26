using Autofac;
using AutoMapper;
using TransactionFlow.BillingSystem.Services.Abstraction;
using TransactionFlow.BillingSystem.Services.Concrete;
using TransactionFlow.BillingSystem.Utilities.AutoMapper;
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
        
        builder.RegisterType<EfIdempotencyDal>().As<IIdempotencyDal>();
        builder.RegisterType<IdempotencyManager>().As<IIdempotencyManager>();
        
        //Services
        builder.RegisterType<TransferService>().As<ITransferService>();
        builder.RegisterType<AccountService>().As<IAccountService>();
        builder.RegisterType<TransactionService>().As<ITransactionService>();
        builder.RegisterType<IdempotencyService>().As<IIdempotencyService>();
        
        //Modules
        builder.Register(context => new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AutoMapperProfile>();
        })).AsSelf().SingleInstance();

        builder.Register(c => c.Resolve<MapperConfiguration>().CreateMapper(c.Resolve))
            .As<IMapper>()
            .InstancePerLifetimeScope();
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
            .Build();
        builder.RegisterInstance(configuration).As<IConfiguration>().SingleInstance();
    }
}