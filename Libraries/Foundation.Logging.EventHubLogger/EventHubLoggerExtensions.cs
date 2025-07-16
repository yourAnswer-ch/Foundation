using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Foundation.Logging.EventHubLogger;

public static class EventHubLoggerExtensions
{
    public static ILoggingBuilder AddEventHubLogger(this ILoggingBuilder builder)
    {
        builder.Services.AddOptions<EventHubLoggerOptions>()
            .BindConfiguration("Logging:EventHub")
            .ValidateOnStart();

        builder.Services.AddHostedService<MessagePublisher>();
        builder.Services.AddSingleton<IMessageQueue, MessageQueue>();
        builder.Services.AddAzureClients(b =>
        {
            b.AddClient<EventHubProducerClient, EventHubLoggerOptions>((options, credential, provider) =>
            {
                var o = provider.GetRequiredService<IOptions<EventHubLoggerOptions>>().Value;                
                return new EventHubProducerClient(o.FullyQualifiedNamespace, o.EventHubName, credential);
            }).WithName("EventHubLogger");
        });

        builder.Services.AddSingleton<ILoggerProvider, EventHubLoggerProvider>();
        return builder;
    }

    public static ILoggingBuilder AddCustomLogger(this ILoggingBuilder builder, Action<EventHubLoggerOptions> configure)
    {
        builder.Services.AddSingleton<ILoggerProvider, EventHubLoggerProvider>();
        builder.Services.Configure(configure);
        return builder;
    }

    public static IDisposable? AddCorrelationId(this ILogger logger, string correlationId)
    {
        return logger.BeginScope(new CorrelationContext(correlationId));
    }
}