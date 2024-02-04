using log4net;

namespace TransactionFlow.Core.CrossCuttingConcerns.Logging.Log4Net.Loggers;

public class FileLogger:LoggerService
{
    public FileLogger() : base(LogManager.GetLogger("FileLogger"))
    {
        
    }
}