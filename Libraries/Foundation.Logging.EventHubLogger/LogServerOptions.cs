using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Foundation.Logging.EventHubLogger;

public class LogServerOptions
{
    public string ConnectionString { get; set; }

    public string AppName { get; set; }

    public LogLevel MinLevel { get; set; }

    public void Update(IConfiguration config)
    {
        SetValue(config, "LogServer:ConnectionString", v => ConnectionString = v);
        SetValue(config, "LogServer:AppName", v => AppName = v);
        SetValue(config, "LogServer:MinLevel", v => MinLevel = GetLogLevel(v, MinLevel));
    }

    private static LogLevel GetLogLevel(string value, LogLevel defaultLogLevel)
    {
        return Enum.TryParse(value, true, out LogLevel level) ? level : defaultLogLevel;            
    }

    private static void SetValue(IConfiguration config, string key, Action<string> set)
    {
        var value = config[key];
        if (!string.IsNullOrEmpty(value))
            set?.Invoke(value);
    }

    public LogServerOptions()
    {
        ConnectionString = "UseDevelopmentStorage=true";
        AppName = "Default";
        MinLevel = LogLevel.Information;
    }
}