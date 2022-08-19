using Azure.Identity;
using Foundation.Configuration.KeyVault;
using Foundation.Hosting.Info;
using Foundation.Hosting.Kestrel.CertBinding;
using Foundation.Logging.EventHubLogger;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.WebHost.ConfigureKestrel(o =>
{
    o.ConfigureBindings();
});


builder.Configuration.AddAzureKeyVault();

//builder.Services.AddAzureClients(builder =>
//{
//    // Register blob service client and initialize it using the KeyVault section of configuration
//    builder.AddSecretClient(Configuration.GetSection("KeyVault"))
//        // Set the name for this client registration
//        .WithName("NamedBlobClient")
//        // Set the credential for this client registration
//        .WithCredential(new ClientSecretCredential("<tenant_id>", "<client_id>", "<client_secret>"))
//        // Configure the client options
//        .ConfigureOptions(options => options.Retry.MaxRetries = 10);

//});


builder.Services.AddAzureClients(builder =>
{
    builder.AddSecretClient(new Uri("http://my.keyvault.com"));
    builder.AddCertificateClient(new Uri("https://kv-fd-certificates.vault.azure.net/")).WithName("KV-FD-Certificates");
    //builder.AddEventHubProducerClient(opts.ConnectionString).WithName("EventHubLogger");
});


builder.Logging.AddEventHubLogger();





builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.RegisterLifetimeLogger();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
