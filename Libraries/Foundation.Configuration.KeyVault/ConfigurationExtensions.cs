using Azure.Core;
using Azure.Core.Diagnostics;
using Azure.Identity;
using Foundation.Logging.EventHubLogger.Configuration;
using Microsoft.Extensions.Configuration;

namespace Foundation.Configuration.KeyVault;

public static class ConfigurationExtensions
{
    public static IConfigurationBuilder AddAzureKeyVault(this IConfigurationBuilder builder, TokenCredential? credentials = null)
    {
        var config = builder.Build().GetKeyVaultConfiguration();

        if (config?.KeyVaultUrl == null)
            throw new ArgumentException("Configuration - KeyVault host url not configured. Local config files needs to be added bevore KeyVault secres source.");

        using AzureEventSourceListener listener = AzureEventSourceListener.CreateConsoleLogger();

        return builder.AddAzureKeyVault(
            new Uri(config.KeyVaultUrl),
            credentials ?? new DefaultAzureCredential());
    }
}