using Foundation.ServiceBuilder.AzureDefault;
using Foundation.Processing.Pipeline;
using Foundation.Processing.Pipeline.Abstractions;
using Foundation.Azure.CertManager.Core.Configuration;
using Foundation.Azure.CertManager.Core.Steps;
using Foundation.Azure.CertManager.Core;
using Foundation.ServiceBuilder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Certes;

var stack = Stack.Create
    .AddDefaultConfiguration()
    .AddDefaultLoggingWithoutEventHubLogger()
    .AddServices(s =>
    {
        s.AddAzureClients(e =>
        {
            e.AddCertificateClient(new Uri("https://kv-main.vault.azure.net/"));
        });

        s.AddMemoryCache();
        
        s.AddPipeline(builder =>
        {
            builder.AddCommand<AzureCheckIfIsExpired>();
            builder.AddCommand<LetsEncryptCreateAccount>();
            builder.AddCommand<LetsEncryptCreateOrder>();
            builder.AddCommand<LetsEncryptAuthorizeDns>();
            builder.AddCommand<AzureCreateTxtRecord, AzureRemoveTxtRecord>();
            builder.AddCommand<LetsEncryptValidate>();
            builder.AddCommand<LetsEncryptDownloadCert>();
            builder.AddCommand<AzureStoreCert>();
            builder.AddCommand<AzureRemoveTxtRecord>();
            //builder.AddCommand<AzureFrontDoor>();
            builder.AddExceptionFormater<AcmeRequestException>(Formator.Exception);
        });

    });

var provider = stack.Build();

var pipeline = provider.GetRequiredService<IPipeline>();


var log = provider.GetRequiredService<ILoggerFactory>().CreateLogger("CertManager");
var config = provider.GetRequiredService<IConfiguration>().GetCertManagerConfig();

foreach (var domain in config.Certificates)
{
    log.LogInformation($"Process domain: {domain.DomainName} resource group: {domain.ResourceGroup}");

    var context = new Context();

    var success = await pipeline.ExecuteAsync(new { 
        Context = context, 
        Domain = domain 
    });       
}