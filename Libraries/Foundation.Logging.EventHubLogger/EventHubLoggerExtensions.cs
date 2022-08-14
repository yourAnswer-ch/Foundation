using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Foundation.Logging.EventHubLogger;

public static class EventHubLoggerExtensions
{
    public static ILoggingBuilder AddEventHubLogger(this ILoggingBuilder builder)
    {
        builder.AddConfiguration();

        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, EventHubLoggerProvider>());
        LoggerProviderOptions.RegisterProviderOptions<EventHubLoggerOptions, EventHubLoggerProvider>(builder.Services);

        return builder;
    }

    public static ILoggingBuilder AddEventHubLogger(this ILoggingBuilder builder, Action<EventHubLoggerOptions> options)
    {
        builder.AddConfiguration();

        builder.Services.Configure(options);
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, EventHubLoggerProvider>());        

        return builder;
    }

    private static void WriteLoggerParameters(this ILoggerFactory factory, string message)
    {
        var log = factory.CreateLogger(nameof(EventHubLogger));
        log?.LogInformation(message);
    }

    public static IDisposable AddCorrelationId(this ILogger logger, string correlationId)
    {
        return logger.BeginScope(new CorrelationContext(correlationId));
    }
}