// See https://aka.ms/new-console-template for more information

using Foundation.CosmosDb;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


Console.WriteLine($"New guid ID: {Id.NewBase62Id}");

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

Console.WriteLine(configuration.GetDebugView());

var services = new ServiceCollection();

services.AddCosmosDb(o => configuration.Bind("Azure:CosmosDB:Flowcpt", o));
services.AddCosmosDbContainer<TestContainer>();


IServiceProvider provider = services.BuildServiceProvider();


var db = provider.GetRequiredService<TestContainer>();
var conatiner = await db.GetOrCreateContainer();

Console.WriteLine("Hello, World!");



public class TestContainer(ICosmosDb db) : CosmosContainer(db)
{
    protected override ContainerProperties CreateContainerProperties()
    {
        return new ContainerProperties
        {
            Id = "TestContainer",
            PartitionKeyPath = "/partitionId"
        };
    }
}