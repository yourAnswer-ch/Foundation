using Foundation.Configuration.KeyVault;
using Microsoft.Extensions.Configuration;

namespace Foundation.Configuration.Defaults;

public static class ConfigurationExtension 
{
    public static void AddDefaultConfiguration(this IConfigurationBuilder builder)
    {
        var environment = Environment.GetEnvironmentVariable("ENVIRONMENT");

        if (string.IsNullOrWhiteSpace(environment))
            environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

        if (string.IsNullOrWhiteSpace(environment))
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        builder.AddJsonFile("appsettings.json", false);
            
        if (!string.IsNullOrWhiteSpace(environment))
            builder.AddJsonFile($"appsettings.{environment}.json", true);

        builder.AddEnvironmentVariables();
        builder.AddAzureKeyVault();        
    }
}