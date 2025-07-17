using System.Collections.Concurrent;
using System.Runtime.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Foundation.Logging.EventHubLogger;

[UnsupportedOSPlatform("browser")]
[ProviderAlias("EventHub")]
public class EventHubLoggerProvider(IServiceProvider provider, IOptions<EventHubLoggerOptions> options)
    : ILoggerProvider
{
    private readonly EventHubLoggerExternalScopeProvider _scopeProvider = new();
    private readonly ConcurrentDictionary<string, EventHubLogger> _loggers = new();
  
    private readonly IMessageQueue _queue = provider.GetRequiredService<IMessageQueue>();
    private readonly EventHubLoggerOptions _options = options.Value;

    public ILogger CreateLogger(string name)
    {
        return _loggers.GetOrAdd(name, 
            n => new EventHubLogger(n, _options.AppName, _options.MinLogLevelToSend, _scopeProvider, _queue));
    }

    public void Dispose()
    {
        // Dispose of any resources if necessary
    }
}
