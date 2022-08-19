using Foundation.Azure.ContainerRegistry.Retention.Core;
using Foundation.Processing.Pipeline;
using Foundation.Processing.Pipeline.Abstractions;
using Foundation.Logging.EventHubLogger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Azure;
using Azure.Containers.ContainerRegistry;
using Azure.Identity;
using Microsoft.Extensions.DependencyInjection;
using Foundation.Configuration.KeyVault;
using Foundation.ServiceBuilder;
using Microsoft.Extensions.Logging;
using Foundation.ServiceBuilder.AzureDefault;

var stack = DefaultAzureStack.Create
    .AddConfiguration()
    .AddLogging()
    .AddServices(s =>
{
    s.AddAzureClients(e =>
    {
        e.AddContianerRegistry();
    });

    s.AddPipeline(builder =>
    {
        builder.AddCommand<ClearContainers>();
    });
});

var provider = stack.Build();




//var configBuilder = new ConfigurationBuilder();
//configBuilder.AddJsonFile("appsettings.json");
//configBuilder.AddAzureKeyVault();
//var config = (IConfiguration)configBuilder.Build();

//var services = new ServiceCollection();

//services.AddSingleton(config);

//services.AddLogging(l => {    
//    l.AddConfiguration(config.GetSection("Logging"));
//    l.AddConsole();
//    l.AddEventHubLogger();
//});

//services.AddAzureClients(e =>
//{
//    e.AddClient<ContainerRegistryClient, ContainerRegistryClientOptions>((o) =>
//    {
//        var endpoint = new Uri("https://ffdev.azurecr.io");
//        o.Audience = ContainerRegistryAudience.AzureResourceManagerPublicCloud;
        
//        return new ContainerRegistryClient(endpoint, new DefaultAzureCredential(), o);
//    });   
//});

//services.AddPipeline(builder =>
//{
//    builder.AddCommand<ClearContainers>();    
//});

//var provider = services.BuildServiceProvider();

var pipeline = provider.GetRequiredService<IPipeline>();

await pipeline.ExecuteAsync(new { url = new Uri("https://wikipedia.org") });

