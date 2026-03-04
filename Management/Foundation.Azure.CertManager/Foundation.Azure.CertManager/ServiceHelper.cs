using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Foundation.Azure.CertManager;

public static class ServiceHelper
{
    public static async Task<CancellationTokenSource> StartBackgroundServices(this IServiceProvider provider)
    {
        var token = new CancellationTokenSource();
        var log = provider.GetRequiredService<ILoggerFactory>().CreateLogger("CertManager");
        var services = provider.GetServices<IHostedService>();
        
        foreach (var service in services)
        {
            log.LogInformation($"Starting BackgroundService: {service.GetType().Name}");
            await service.StartAsync(token.Token);
        }

        return token;
    }
}