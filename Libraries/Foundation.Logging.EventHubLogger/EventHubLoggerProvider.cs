using System.Collections.Concurrent;
using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Foundation.Logging.EventHubLogger;

[UnsupportedOSPlatform("browser")]
[ProviderAlias("EventHub")]
public class EventHubLoggerProvider : ILoggerProvider
{
    private readonly EventHubLoggerOptions _options;
    private readonly IMessagePump _messagePump;
    private readonly Func<string, LogLevel, bool>? _filter;
    private readonly EventHubLoggerExternalScopeProvider _scopeProvider;
    private readonly ConcurrentDictionary<string, EventHubLogger> _loggers = new();

    public ILogger CreateLogger(string name)
    {
        return _loggers.GetOrAdd(name, CreateLoggerImplementation);
    }

    private EventHubLogger CreateLoggerImplementation(string name)
    {
        return new EventHubLogger(_options, name, _filter, _scopeProvider, _messagePump);
    }

    public EventHubLoggerProvider(IOptions<EventHubLoggerOptions> options)
    {
        _options = options.Value;       
        _scopeProvider = new EventHubLoggerExternalScopeProvider();
        _messagePump = new MessagePump(_options);
        _messagePump.Start();
    }

    void IDisposable.Dispose() {
        GC.SuppressFinalize(this);
    }
}
