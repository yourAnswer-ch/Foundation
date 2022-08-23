using Foundation.Configuration.Defaults;
using Foundation.Hosting.Info;
using Foundation.Logging.EventHubLogger;
using Foundation.Processing.StorageQueue;
using Microsoft.Extensions.Azure;


var builder = Host.CreateDefaultBuilder(args);

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

    s.AddTransient<IMessageHandler, MessageHandler>();
    s.AddTransient<QueueReciver>();
    s.AddHostedService<QueueProcessorService>();
});

builder.Build().RegisterLifetimeLogger().Start();
