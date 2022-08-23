using Microsoft.Extensions.DependencyInjection;
using Foundation.Azure.CertManager.Core.Slack;

namespace Foundation.Azure.CertManager.Core;

public static class CertManagerServiceExtension
{
    public static void AddCertManager(this IServiceCollection services)
    {
        services.AddSlackBot();
        //services.AddSingleton<ICertManagementService, CertManagementService>();
    }
}
