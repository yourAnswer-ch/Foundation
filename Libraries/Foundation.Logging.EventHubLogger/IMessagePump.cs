using Foundation.Logging.EventHubLogger.Interface;

namespace Foundation.Logging.EventHubLogger;

public interface IMessagePump
{
    void Append(LogEntry logEntry);

    void Start();

    void Stop();
}