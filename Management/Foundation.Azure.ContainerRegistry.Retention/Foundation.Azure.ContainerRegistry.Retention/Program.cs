using Foundation.Azure.ContainerRegistry.Retention.Core;
using Foundation.Processing.Pipeline;
using Foundation.Processing.Pipeline.Abstractions;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Foundation.ServiceBuilder.AzureDefault;
using Microsoft.Extensions.Configuration;

var stack = DefaultAzureStack.Create
    .AddConfiguration(c =>
    {
        c.AddEnvironmentVariables();
    })
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

var pipeline = provider.GetRequiredService<IPipeline>();

await pipeline.ExecuteAsync(null);

