// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Attributes;
using Foundation.CosmosDb;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;




//BenchmarkRunner.Run<TestClass>();

var instance = new TestClass();

instance.GlobalSetup();

await instance.RunTest7();



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

        var result = container.QueryItemsAsync<JObject>(query);

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

        var result = container.QueryItemsAsync<JObject>(query);

        var list = await result.ToListAsync();
    }

    [Benchmark]
    public async Task RunTest4()
    {
        var result = container.QueryItemsAsync<JObject>(
            "SELECT * FROM c WHERE c.id = @id",
            new Dictionary<string, object> { { "id", "0529ddfc-c723-416d-a2c4-a5969aa7c5c1" } });

        var list = await result.ToListAsync();

        foreach(var doc in list)
        {
            Console.WriteLine(doc);
        }
    }

    [Benchmark]
    public async Task RunTest5()
    {
        var result = container.QueryItemsAsync<JObject>(
            "SELECT * FROM c WHERE c.id = @id",
            new { id = "0529ddfc-c723-416d-a2c4-a5969aa7c5c1" });

        var list = await result.ToListAsync();

        foreach (var doc in list)
        {
            Console.WriteLine(doc);
        }
    }

    [Benchmark]
    public async Task RunTest6()
    {
        var result = await container.GetItemAsync<JObject>("0529ddfc-c723-416d-a2c4-a5969aa7c5c1", "3K6ehNLhnRsPfWhzjugmea_InventoryItems");
               
        Console.WriteLine(result);        
    }

    [Benchmark]
    public async Task RunTest7()
    {
        var result = await container.GetItemAsync<JObject>("SELECT * FROM c WHERE c.name = @name", new { Name = "New Item" });

        Console.WriteLine(result);
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