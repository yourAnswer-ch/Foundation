using Foundation.Logging.EventHubLogger.Interface;
using System.Collections.Concurrent;

namespace Foundation.Logging.EventHubLogger;

internal interface IMessageQueue
{
    void Append(LogEntry logEntry);

    BlockingCollection<LogEntry> Messages { get; }
}
