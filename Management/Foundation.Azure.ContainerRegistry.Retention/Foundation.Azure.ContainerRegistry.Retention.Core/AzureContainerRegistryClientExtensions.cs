using Azure.Containers.ContainerRegistry;
using Azure.Identity;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Foundation.Azure.ContainerRegistry.Retention.Core
{
    public static class AzureContainerRegistryClientExtensions
    {
        public static void AddContianerRegistry(this AzureClientFactoryBuilder builder)
        {
            builder.AddClient<ContainerRegistryClient, ContainerRegistryClientOptions>((o, s) =>
            {
                var config = s.GetRequiredService<IConfiguration>();
                var endpoint = new Uri(config.GetValue<string>("Azure:ContainerRegistry:Endpoint"));
                o.Audience = ContainerRegistryAudience.AzureResourceManagerPublicCloud;

                return new ContainerRegistryClient(endpoint, new DefaultAzureCredential(), o);
            });
        }

    }
}
