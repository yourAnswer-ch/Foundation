// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Foundation.CosmosDb;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;


//Console.WriteLine($"New guid ID: {Id.NewBase62Id}");

//var configuration = new ConfigurationBuilder()
//    .AddUserSecrets<Program>()
//    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//    .Build();

//Console.WriteLine(configuration.GetDebugView());

//var services = new ServiceCollection();

//services.AddCosmosDb(o => configuration.Bind("Azure:CosmosDB:Flowcpt", o));
//services.AddCosmosDbContainer<TestContainer>();


//IServiceProvider provider = services.BuildServiceProvider();


//var db = provider.GetRequiredService<TestContainer>();
//var conatiner = await db.GetOrCreateContainer();

//var container = provider.GetRequiredService<TestContainer>();

//// Test 1
//var query = new QueryDefinition("SELECT * FROM c");

//var result = container.QueryAsync<JObject>(query);

//await foreach (var item in result)
//{
//    Console.WriteLine(item);
//}


// Test 2
//BenchmarkRunner.Run()

//var container2 = await container.GetOrCreateContainer();
//var query2 = new QueryDefinition("SELECT * FROM c");
//var result2 = container2.GetItemQueryIterator<JObject>(query2);

//while (result2 != null && result2.HasMoreResults)
//{
//    foreach (JObject document in await result2.ReadNextAsync())
//    {
//        Console.WriteLine(document);
//    }
//}

BenchmarkRunner.Run<TestClass>();




Console.WriteLine("Hello, World!");

[MemoryDiagnoser]
public class TestClass
{
    TestContainer container;

    [GlobalSetup]
    public void GlobalSetup()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        Console.WriteLine(configuration.GetDebugView());

        var services = new ServiceCollection();

        services.AddCosmosDb(o => configuration.Bind("Azure:CosmosDB:Flowcpt", o));
        services.AddCosmosDbContainer<TestContainer>();


        IServiceProvider provider = services.BuildServiceProvider();

        container = provider.GetRequiredService<TestContainer>();
    }

    [Benchmark]
    public async Task RunTest1()
    {
        var query = new QueryDefinition("SELECT * FROM c");

        var result = container.QueryAsync<JObject>(query);

        var list = new List<JObject>();

        await foreach (var item in result)
        {
            list.Add(item);
            //Console.WriteLine(item);
        }
    }

    [Benchmark]
    public async Task RunTest2()
    {
        var container2 = await container.GetOrCreateContainer();
        var query2 = new QueryDefinition("SELECT * FROM c");
        var result2 = container2.GetItemQueryIterator<JObject>(query2);

        var list = new List<JObject>();

        while (result2 != null && result2.HasMoreResults)
        {
            foreach (JObject document in await result2.ReadNextAsync())
            {
                list.Add(document);
                //Console.WriteLine(document);
            }
        }
    }

    [Benchmark]
    public async Task RunTest3()
    {
        var query = new QueryDefinition("SELECT * FROM c");

        var result = container.QueryAsync<JObject>(query);

        var list = await result.ToListAsync();
    }
}



public class TestContainer(ICosmosDb db) : CosmosContainer(db)
{
    protected override ContainerProperties CreateContainerProperties()
    {
        return new ContainerProperties
        {
            Id = "Document",
            PartitionKeyPath = "/partitionId"
        };
    }
}