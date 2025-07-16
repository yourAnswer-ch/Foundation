using Foundation.Logging.EventHubLogger.Interface;
using System.Collections.Concurrent;

namespace Foundation.Logging.EventHubLogger;

internal class MessageQueue : IMessageQueue
{
    private readonly BlockingCollection<LogEntry> _messages = [];

    public BlockingCollection<LogEntry> Messages => _messages;

    public void Append(LogEntry logEntry)
    {
        _messages.Add(logEntry);
    }
}
