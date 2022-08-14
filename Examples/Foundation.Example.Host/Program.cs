using Azure.Identity;
using Foundation.Hosting.Info;
using Foundation.Logging.EventHubLogger;
using Foundation.Processing.StorageQueue;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddEventHubLogger();

builder.Configuration.AddAzureKeyVault(new Uri("https://ff-dev.vault.azure.net/"), new DefaultAzureCredential());

builder.Services.AddAzureClients(builder =>
{
    //builder.AddSecretClient(new Uri("http://my.keyvault.com"));
    //builder.AddCertificateClient(new Uri("https://kv-fd-certificates.vault.azure.net/")).WithName("KV-FD-Certificates");
});

builder.Services.AddHostedService<QueueProcessorService>();

var app = builder.Build();

app.RegisterLifetimeLogger();

app.MapGet("/", () => "Hello World!");

app.Run();
