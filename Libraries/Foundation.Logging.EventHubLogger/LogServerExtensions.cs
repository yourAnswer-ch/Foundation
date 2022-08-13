using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Azure;

namespace Foundation.Logging.EventHubLogger;

public static class LogServerExtensions
{
    public static ILoggingBuilder AddEventHubLogger(this ILoggingBuilder builder, Action<LogServerOptions>? options = null)
    {
        builder.AddConfiguration();

        builder.Services.Configure(options);
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, LogServerProvider>());        

        return builder;
    }

    public static ILoggingBuilder AddEventHubLogger(this ILoggingBuilder builder, IMessagePump messagePump, Action<LogServerOptions>? options = null)
    {
        builder.AddConfiguration();

        var opts = new LogServerOptions();

        options?.Invoke(opts);

        var logServerProvider = new LogServerProvider(opts, messagePump);

        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider>(logServerProvider));            

        return builder;
    }

    public static ILoggerFactory AddEventHubLogger(this ILoggerFactory factory, Action<LogServerOptions>? options = null, Func<string, LogLevel, bool>? filter = null)
    {
        var logServerOptions = new LogServerOptions();
        options?.Invoke(logServerOptions);

        factory.AddProvider(new LogServerProvider(logServerOptions, filter));
        factory.WriteLoggerParameters($"Add LogServerLogger app name: {logServerOptions.AppName} Filter: {logServerOptions.MinLevel}.");
        return factory;
    }


    private static void WriteLoggerParameters(this ILoggerFactory factory, string message)
    {
        var log = factory.CreateLogger(nameof(LogServerLogger));
        log?.LogInformation(message);
    }

    public static IDisposable AddCorrelationId(this ILogger logger, string correlationId)
    {
        return logger.BeginScope(new CorrelationContext(correlationId));
    }
}