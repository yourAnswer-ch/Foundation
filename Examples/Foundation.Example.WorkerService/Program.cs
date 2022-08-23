using Foundation.Configuration.KeyVault;
using Foundation.Hosting.Info;
using Foundation.Logging.EventHubLogger;
using Foundation.Processing.StorageQueue;
using Microsoft.Extensions.Azure;


var builder = Host.CreateDefaultBuilder(args);

var environment = Environment.GetEnvironmentVariable("ENVIRONMENT");

if (string.IsNullOrWhiteSpace(environment))
    environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

builder.ConfigureHostConfiguration(c =>
{
    c.AddDefaultConfiguration();
});

builder.ConfigureLogging(l =>
{
    l.AddEventHubLogger();
});

builder.ConfigureServices(s => {
    s.AddAzureClients(builder =>
    {
        //builder.AddSecretClient(new Uri("http://my.keyvault.com"));
        //builder.AddCertificateClient(new Uri("https://kv-fd-certificates.vault.azure.net/")).WithName("KV-FD-Certificates");
    });

    s.AddHostedService<QueueProcessorService>();
});

builder.Build().RegisterLifetimeLogger().Start();
