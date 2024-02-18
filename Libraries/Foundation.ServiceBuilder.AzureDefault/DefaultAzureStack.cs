using Foundation.Configuration.KeyVault;
using Foundation.Logging.EventHubLogger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Foundation.ServiceBuilder.AzureDefault;

public static class DefaultSettings
{
    public static IStack AddDefaultConfiguration(this IStack stack)
    {
        return stack.AddDefaultConfiguration(null);
    }

    public static IStack AddDefaultConfiguration(this IStack stack, Action<IConfigurationBuilder>? builder)
    {
        var environment = Environment.GetEnvironmentVariable("ENVIRONMENT");

        if (string.IsNullOrWhiteSpace(environment))
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        return stack.AddConfiguration(b =>
        {
            b.AddJsonFile("appsettings.json", false);
            if (!string.IsNullOrWhiteSpace(environment))
                b.AddJsonFile($"appsettings.{environment}.json", true);                
            b.AddAzureKeyVault();
            builder?.Invoke(b);
        });
    }

    public static IStack AddDefaultLogging(this IStack stack)
    {
        return stack.AddDefaultLogging(_ => { });
    }

    public static IStack AddDefaultLogging(this IStack stack, Action<ILoggingBuilder, IConfiguration>? builder)
    {
        return stack.AddLogging((b, c) =>
        {
            b.AddConsole();                
            b.AddEventHubLogger();
            b.AddTraceSource("Sherlock");
            b.AddConfiguration(c.GetSection("Logging"));
            builder?.Invoke(b, c);
        });
    }

    public static IStack AddDefaultLogging(this IStack stack, Action<ILoggingBuilder>? builder)
    {
        return stack.AddLogging((b, c) =>
        {
            b.AddConsole();
            b.AddEventHubLogger();
            b.AddTraceSource("Sherlock");
            b.AddConfiguration(c.GetSection("Logging"));
            builder?.Invoke(b);
        });
    }
}
