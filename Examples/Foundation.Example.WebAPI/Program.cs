using Foundation.Configuration.KeyVault;
using Foundation.Hosting.Info;
using Foundation.Hosting.Kestrel.CertBinding;
using Foundation.Logging.EventHubLogger;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.WebHost.ConfigureKestrel(o =>
{
    o.ConfigureBindings();
});

builder.Configuration.AddAzureKeyVault();

builder.Services.AddAzureClients(builder =>
{    
    builder.AddCertificateClient(new Uri("https://kv-fd-certificates.vault.azure.net/")).WithName("KV-FD-Certificates");
});

builder.Services.AddCertService();

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
