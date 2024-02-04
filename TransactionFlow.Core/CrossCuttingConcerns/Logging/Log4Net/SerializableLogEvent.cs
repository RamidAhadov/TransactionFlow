using log4net.Core;

namespace TransactionFlow.Core.CrossCuttingConcerns.Logging.Log4Net;

[Serializable]
public class SerializableLogEvent
{
    private LoggingEvent _loggingEvent;

    public SerializableLogEvent(LoggingEvent loggingEvent)
    {
        _loggingEvent = loggingEvent;
    }

    public string UserName => _loggingEvent.UserName;
    public object MessageObject => _loggingEvent.MessageObject;

}