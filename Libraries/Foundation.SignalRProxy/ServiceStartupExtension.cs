using Foundation.SignalRRelay.Configurations;

namespace Foundation.SignalRRelay;

public static class ServiceStartupExtension
{
    public static IServiceCollection AddSignalRRelay(this IServiceCollection services, IConfiguration configuration)
    {        
        services.Configure<EventHubOptions>(configuration.GetSection("Azure:EventHub:SignalRRelay"));
        services.AddHostedService<EventHubListenerService>();
        services.AddSignalR();
        return services;
    }

    public static WebApplication UseSignalRRelay(this WebApplication app)
    {
        app.MapHub<ObjectUpdateHub>("/updates");
        return app;
    }
}