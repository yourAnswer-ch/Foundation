using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Foundation.Hosting.Kestrel.CertBinding.Configuration;

internal static class ConfigExtension
{
    public static KestrelConfig GetKestrelConfig(this IConfiguration config, string key = "Hosting:Kestrel")
    {
        return config.GetSection(key).Get<KestrelConfig>();
    }

    public static void AddConfigurationBinding(this IServiceCollection services)
    {
        services.AddTransient(p => p.GetRequiredService<IConfiguration>().GetKestrelConfig());
    }
}