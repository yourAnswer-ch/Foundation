using Foundation.Azure.ContainerRegistry.Retention.Core;
using Foundation.Processing.Pipeline;
using Foundation.Processing.Pipeline.Abstractions;

var services = new ServiceCollection();

services.AddLogging(l => l.AddConsole());

services.AddPipeline(builder =>
{
    builder.AddCommand<ClearContainers>();
    builder.AddCommand<WriteConsole>();
});


var provider = services.BuildServiceProvider();

var pipeline = provider.GetRequiredService<IPipeline>();

await pipeline.ExecuteAsync(provider, new { url = new Uri("https://wikipedia.org") });

//var builder = WebApplication.CreateBuilder(args);


//var app = builder.Build();

//app.MapGet("/", () => "Hello World!");

//app.Run();


//await pipeline.ExecuteAsync(app.Services);