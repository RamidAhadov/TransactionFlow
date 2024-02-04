using log4net;
using log4net.Config;

namespace TransactionFlow.BillingSystem.Extensions;

public static class Log4NetExtension
{
    public static void AddLog4Net(this IServiceCollection services)
    {
        XmlConfigurator.Configure(new FileInfo("log4net.config"));                    
        services.AddSingleton(LogManager.GetLogger(typeof(Program)));
    }
}