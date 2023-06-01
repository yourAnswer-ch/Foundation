using Foundation.Logging.EventHubLogger.Interface;

namespace CloudLogger.Filtering;

internal interface ICondition
{
    bool Match(LogEntry logEntry);
}