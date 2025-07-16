using System.Collections.Concurrent;
using System.Runtime.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Foundation.Logging.EventHubLogger;

[UnsupportedOSPlatform("browser")]
[ProviderAlias("EventHub")]
public class EventHubLoggerProvider : ILoggerProvider
{
    private readonly EventHubLoggerExternalScopeProvider _scopeProvider = new EventHubLoggerExternalScopeProvider();
    private readonly ConcurrentDictionary<string, EventHubLogger> _loggers = new();
  
    private readonly IMessageQueue _queue;
    private readonly EventHubLoggerOptions _options;

    public EventHubLoggerProvider(IServiceProvider provider, IOptions<EventHubLoggerOptions> options)
    {
        if(options.Value == null)
            throw new ArgumentNullException(nameof(options), "Event hub configuration is missing.");
        _options = options.Value;
        _queue = provider.GetRequiredService<IMessageQueue>();        
    }

    public ILogger CreateLogger(string name)
    {
        return _loggers.GetOrAdd(name, (n) =>
        {            
            return new EventHubLogger(_options.AppName, n, _options.MinLogLevelToSend, _scopeProvider, _queue);
        });
    }

    public void Dispose()
    {
        // Dispose of any resources if necessary
    }
}
