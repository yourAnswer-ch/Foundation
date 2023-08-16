using Microsoft.Extensions.DependencyInjection;

namespace Foundation.Hosting.Kestrel.CertBinding;

public static class ServiceExtension
{
    public static void AddCertService(this IServiceCollection services)
    {
        services.AddHostedService<CertRefreshService>();
        services.AddSingleton<CertificationStore>();
    }

    //Only to satisfy compiler
    public static void Main(string[] args) { }
}