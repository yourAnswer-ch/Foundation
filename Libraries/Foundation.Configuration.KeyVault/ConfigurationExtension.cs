using Microsoft.Extensions.Configuration;

namespace Foundation.Logging.EventHubLogger.Configuration;

public static class ConfigurationExtension
{
    public static KeyVaultConfiguration GetKeyVaultConfiguration(this IConfiguration config, string key = "Secrets")
    {
        return config.GetSection(key).Get<KeyVaultConfiguration>();
    }
}