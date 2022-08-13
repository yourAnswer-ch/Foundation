using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Foundation.Logging.EventHubLogger;

public class LogServerProvider : ILoggerProvider
{
    private readonly LogServerOptions _options;
    private readonly IMessagePump _messagePump;
    private readonly Func<string, LogLevel, bool>? _filter;
    private readonly LogServerExternalScopeProvider _scopeProvider;
    private readonly ConcurrentDictionary<string, LogServerLogger> _loggers = new();

    public ILogger CreateLogger(string name)
    {
        return _loggers.GetOrAdd(name, CreateLoggerImplementation);
    }

    private LogServerLogger CreateLoggerImplementation(string name)
    {
        return new LogServerLogger(_options, name, _filter, _scopeProvider, _messagePump);
    }

    public LogServerProvider(IOptions<LogServerOptions> options)
    {
        _options = options.Value;
        _scopeProvider = new LogServerExternalScopeProvider();
        _messagePump = new MessagePump(_options);
        _messagePump.Start();
    }

    internal LogServerProvider(LogServerOptions options, IMessagePump messagePump)
    {
        _filter = null;
        _options = options;
        _scopeProvider = new LogServerExternalScopeProvider();
        _messagePump = messagePump;
        _messagePump.Start();
    }

    internal LogServerProvider(LogServerOptions options, Func<string, LogLevel, bool>? filter)
    {
        _filter = filter;
        _options = options;
        _scopeProvider = new LogServerExternalScopeProvider();
        _messagePump = new MessagePump(_options);
        _messagePump.Start();
    }

    void IDisposable.Dispose() {
        GC.SuppressFinalize(this);
    }
}
