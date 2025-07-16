using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

namespace Foundation.Logging.EventHubLogger;

public class EventHubLoggerOptions
{
    [Required]
    public required string FullyQualifiedNamespace { get; set; }

    [Required]
    public required string EventHubName { get; set; }

    [Required]
    public required string AppName { get; set; }

    public LogLevel MinLogLevelToSend { get; set; } = LogLevel.Information;
}