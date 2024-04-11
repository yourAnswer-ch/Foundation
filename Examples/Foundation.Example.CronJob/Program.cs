// See https://aka.ms/new-console-template for more information
using Foundation.ServiceBuilder;
using Foundation.ServiceBuilder.AzureDefault;
using Microsoft.Extensions.Azure;


var stack = Stack.Create
    .AddDefaultConfiguration()
    .AddDefaultLogging()
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