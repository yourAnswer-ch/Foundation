using Microsoft.Extensions.Logging;

namespace Foundation.Logging.EventHubLogger.Interface;

public class LogEntry
{
    public LogLevel LogLevel { get; set; }

    public int EventId { get; set; }

    public string? EventName { get; set; }

    public string? Host { get; set; }

    public string? App { get; set; }

    public string? Name { get; set; }

    public string? Message { get; set; }

    public DateTime Timestamp { get; set; }

    public string? CorrelationId { get; set; }

    public override string ToString()
    {
        return ToString(false);
    }

    public string ToString(bool showName = false)
    {
        if (showName)
            return $"[{Timestamp:HH:mm:ss.ffff ddMMyy}] {LogLevel,-11}: [{Host}] [{App}] [{Name}] [{CorrelationId}] {Message}";
        else
            return $"[{Timestamp:HH:mm:ss.ffff ddMMyy}] {LogLevel,-11}: [{Host}] [{App}] [{CorrelationId}] {Message}";
    }
}
