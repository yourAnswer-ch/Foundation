using Microsoft.Extensions.Logging;

namespace Foundation.Logging.EventHubLogger;

public class EventHubLoggerExternalScopeProvider : IExternalScopeProvider
{
    public void ForEachScope<TState>(Action<object, TState> callback, TState state)
    {
        throw new NotImplementedException();
    }

    public ICorrelationContext? Context { get; private set; }

    public IDisposable Push(object? state)
    {
        Context = state as ICorrelationContext;
        return new CorrelationScope(this);
    }

    class CorrelationScope(EventHubLoggerExternalScopeProvider provider) : IDisposable
    {
        private bool _isDisposed;

        public void Dispose()
        {
            if (_isDisposed) return;
            provider.Context = null;
            _isDisposed = true;
        }
    }
}
