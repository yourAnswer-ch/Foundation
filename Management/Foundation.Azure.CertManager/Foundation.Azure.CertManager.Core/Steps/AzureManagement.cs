using Foundation.Azure.CertManager.Core.Configuration;
using Foundation.Processing.Pipeline;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Extensions.Configuration;

namespace Foundation.Azure.CertManager.Core.Steps;

public abstract class AzureManagement : Command
{
    protected CertificatesConfig Config;

    protected AzureManagement(IConfiguration config)
    {
        Config = config.GetCertManagerConfig();
    }

    protected IAzure CreateClient()
    {
        var credentials = new AzureCredentials(
            new ServicePrincipalLoginInformation
            {
                ClientId = Config.AdTenant.ClientId,
                ClientSecret = Config.AdTenant.ClientSecret
            },
            Config.AdTenant.TenantId, AzureEnvironment.AzureGlobalCloud);

        var authenticatedAzure = Microsoft.Azure.Management.Fluent.Azure
            .Configure()
            .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
            .Authenticate(credentials);

        return authenticatedAzure.WithDefaultSubscription();
    }
}
