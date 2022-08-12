using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Foundation.Hosting.Info.Configuration;
using Microsoft.Extensions.Configuration;

namespace Foundation.Hosting.Info;

public static class LifetimeLoggerExtension
{
    public static void RegisterLifetimeLogger(this IServiceProvider provider)
    {
        var log = provider.GetLogger();
        var rootConfig = provider.GetService<IConfiguration>();
        var lifetime = provider.GetService<IHostApplicationLifetime>();

        if (lifetime == null)
        {
            log?.LogWarning("LifetimeLogger - IHostApplicationLifetime not found.");
            return;
        }

        if (rootConfig == null)
        {
            log?.LogWarning("LifetimeLogger - IConfiguration not found.");
            return;
        }

        var config = rootConfig.GetInfoConfig();
        lifetime.ApplicationStarted.Register(() => log?.LogStartup(config));
        lifetime.ApplicationStopped.Register(() => log?.LogStop(config));
    }

    private static ILogger? GetLogger(this IServiceProvider provider)
    {
        try
        {
            var type = Assembly.GetEntryAssembly()?.EntryPoint?.DeclaringType ?? typeof(IHostApplicationLifetime);
            return provider.GetService<ILoggerFactory>()?.CreateLogger(type);
        }
        catch
        {
            return null;
        }
    }
}
