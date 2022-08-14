using Microsoft.Extensions.Logging;

namespace Foundation.Logging.EventHubLogger;

public class EventHubLoggerOptions
{
    public string ConnectionString { get; set; }

    public string AppName { get; set; }

    public LogLevel MinLevel { get; set; }

    public EventHubLoggerOptions()
    {
        ConnectionString = "UseDevelopmentStorage=true";
        AppName = "Default";
        MinLevel = LogLevel.Information;
    }
}