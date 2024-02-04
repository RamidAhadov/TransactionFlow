using Autofac;
using TransactionFlow.Business.Abstraction;
using TransactionFlow.Business.Concrete;
using TransactionFlow.DataAccess.Abstraction;
using TransactionFlow.DataAccess.Concrete.EntityFramework;

namespace TransactionFlow.Business.DependencyResolvers.Autofac;

public class AutofacBusinessModule:Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<EfCustomerDal>().As<ICustomerDal>();
        builder.RegisterType<CustomerService>().As<ICustomerService>();
    }
}