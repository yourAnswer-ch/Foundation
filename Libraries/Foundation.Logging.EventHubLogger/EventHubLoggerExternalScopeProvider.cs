using Microsoft.Extensions.Logging;

namespace Foundation.Logging.EventHubLogger;

public class EventHubLoggerExternalScopeProvider : IExternalScopeProvider
{
    private ICorrelationContext? _currentContext;

    public void ForEachScope<TState>(Action<object, TState> callback, TState state)
    {
        throw new NotImplementedException();
    }

    public ICorrelationContext? Context => _currentContext;

    public IDisposable Push(object? state)
    {
        _currentContext = state as ICorrelationContext;
        return new CorrelationScope(this);
    }

    class CorrelationScope : IDisposable
    {
        private readonly EventHubLoggerExternalScopeProvider _provider;
        private bool _isDisposed;

        public CorrelationScope(EventHubLoggerExternalScopeProvider provider)
        {
            _provider = provider;
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _provider._currentContext = null;
                _isDisposed = true;
            }
        }
    }
}
