// See https://aka.ms/new-console-template for more information
using Foundation.ServiceBuilder.AzureDefault;
using Microsoft.Extensions.Azure;


var stack = DefaultAzureStack.Create
    .AddConfiguration()
    .AddLogging()
    .AddServices(s =>
    {
        s.AddAzureClients(e =>
        {
            
        });

        //s.AddPipeline(builder =>
        //{
        //    builder.AddCommand<ClearContainers>();
        //});
    });

var provider = stack.Build();

//Do Code