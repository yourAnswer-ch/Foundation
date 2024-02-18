using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Foundation.ServiceBuilder.AzureDefault;

public static class AlternativeDefaultSettings
{
    public static IStack AddLoggingWithoutEventHubLogger(this IStack stack)
    {
        return stack.AddLoggingWithoutEventHubLogger(_ => { });
    }

    public static IStack AddLoggingWithoutEventHubLogger(this IStack stack, Action<ILoggingBuilder, IConfiguration>? builder)
    {
        return stack.AddLogging((b, c) =>
        {
            b.AddConsole();
            b.AddTraceSource("Sherlock");
            b.AddConfiguration(c.GetSection("Logging"));
            builder?.Invoke(b, c);
        });
    }

    public static IStack AddLoggingWithoutEventHubLogger(this IStack stack, Action<ILoggingBuilder>? builder)
    {
        return stack.AddLogging((b, c) =>
        {
            b.AddConsole();
            b.AddTraceSource("Sherlock");
            b.AddConfiguration(c.GetSection("Logging"));
            builder?.Invoke(b);
        });
    }
}