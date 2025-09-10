using Foundation.Configuration.KeyVault;
using Foundation.Hosting.Info;
using Foundation.Hosting.Kestrel.CertBinding;
using Foundation.Logging.EventHubLogger;
using Microsoft.Extensions.Azure;
using Foundation.Services.ImageProcessor.Core;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(o =>
{
    o.ConfigureBindings();
});

builder.Configuration.AddAzureKeyVault();

builder.Services.AddCertService();

builder.Services.AddImageProcessor();


//builder.Services.AddFirebaseAuthentication(builder.Configuration);

builder.Services.AddAzureClients(builder =>
{
    builder.AddCertificateClient(new Uri("https://kv-fd-certificates.vault.azure.net/")).WithName("KV-FD-Certificates");
});


builder.Services.AddHealthChecks();

builder.Logging.AddEventHubLogger();

var app = builder.Build();

app.UseImageProcessor();
app.MapGet("/", () => "Hello World!");

app.RegisterLifetimeLogger();

app.Run();
