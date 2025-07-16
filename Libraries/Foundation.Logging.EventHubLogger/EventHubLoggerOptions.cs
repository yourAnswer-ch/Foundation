using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

namespace Foundation.Logging.EventHubLogger;

public class EventHubLoggerOptions
{
    [Required]
    public required string FullyQualifiedNamespace { get; init; } 

    public string AppName { get; init; } = "Default";

    public LogLevel MinLevel { get; init; } = LogLevel.Information;
}