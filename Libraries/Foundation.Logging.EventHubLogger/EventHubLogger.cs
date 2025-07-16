using System.Collections;
using System.Diagnostics;
using System.Text;
using Foundation.Logging.EventHubLogger.Interface;
using Microsoft.Extensions.Logging;

namespace Foundation.Logging.EventHubLogger;

public class EventHubLogger : ILogger
{
    private string _name;
    private string _appName;
    private const int Indentation = 2;
    private readonly LogLevel _minLogLevelToSend;
    private readonly EventHubLoggerExternalScopeProvider _scopeProvider;
    private readonly IMessageQueue _messageQueue;

    internal EventHubLogger(
        string name,
        string appName,
        LogLevel minLogLevelToSend,        
        EventHubLoggerExternalScopeProvider scopeProvider,
        IMessageQueue queue)
    {
        _name = name;
        _appName = appName;
        _minLogLevelToSend = minLogLevelToSend;
        _scopeProvider = scopeProvider;
        _messageQueue = queue;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel >= _minLogLevelToSend;
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (logLevel < _minLogLevelToSend)
            return;

        var logEntry = new LogEntry
        {
            App = _appName,
            Name = _name,
            Host = Environment.MachineName,
            EventId = eventId.Id,
            EventName = eventId.Name,
            LogLevel = logLevel,
            Message = GetMessage(state, exception, formatter),
            Timestamp = DateTime.UtcNow,
            CorrelationId = _scopeProvider.Context?.CorrelationId ?? Activity.Current?.Id
        };

        _messageQueue.Append(logEntry);
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return _scopeProvider.Push(state);
    }

    private string GetMessage<TState>(TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var builder = new StringBuilder();

        //if (_scopeProvider.Context!=null)
        //{
        //    builder.Append($"[{_scopeProvider.Context.CorrelationId}] ");
        //}

        if (formatter != null)
        {
            builder.Append(formatter(state, exception));
        }
        else
        {
            if (state is IReadOnlyList<KeyValuePair<string, object>> values)
            {
                FormatLogValues(builder, values, 1, false);
            }
        }

        if (exception != null)
            builder.AppendLine().Append(exception);

        return builder.ToString();
    }

    private void FormatLogValues(StringBuilder builder, IReadOnlyList<KeyValuePair<string, object>> logValues, int level, bool bullet)
    {
        if (logValues == null)
            return;

        var isFirst = true;
        foreach (var kvp in logValues)
        {
            builder.AppendLine();

            if (bullet && isFirst)
                builder.Append(' ', level * Indentation - 1).Append('-');
            else
                builder.Append(' ', level * Indentation);

            builder.Append(kvp.Key).Append(": ");

            switch (kvp.Value)
            {
                case string str:
                    builder.Append(str);
                    break;

                case IEnumerable values:
                    foreach (var value in values)
                    {
                        if (value is IReadOnlyList<KeyValuePair<string, object>> list)
                        {
                            FormatLogValues(builder, list, level + 1, true);
                        }
                        else
                        {
                            builder.AppendLine().Append(' ', (level + 1) * Indentation).Append(values);
                        }
                    }
                    break;

                default:
                    if (kvp.Value is IReadOnlyList<KeyValuePair<string, object>> kvList)
                    {
                        FormatLogValues(builder, kvList, level + 1, false);
                    }
                    else
                    {
                        builder.Append(kvp.Value);
                    }
                    break;
            }
            isFirst = false;
        }
    }
}