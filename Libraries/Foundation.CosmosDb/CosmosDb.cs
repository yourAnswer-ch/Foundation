using Azure.Identity;
using Foundation.CosmosDb.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net;

namespace Foundation.CosmosDb;

public class CosmosDb : ICosmosDb
{
    private Database? _database;
    private CosmosClient? _client;
    private CosmosDbOptions _options;

    internal CosmosClient Client => _client ??= CreateClient(_options);

    public CosmosDb(CosmosDbOptions options)
    {
        _options = options;
    }

    public static CosmosDb Create(IServiceProvider provider)
    {
        var options = provider.GetRequiredService<IOptions<CosmosDbOptions>>().Value;

        return ActivatorUtilities.CreateInstance<CosmosDb>(provider, options);
    }

    public static T Create<T>(IServiceProvider provider) where T : CosmosDb, new()
    {
        var options = provider.GetRequiredService<IOptionsMonitor<CosmosDbOptions>>().Get(typeof(T).Name);

        return ActivatorUtilities.CreateInstance<T>(provider, options);
    }

    public async Task<Database> GetOrCreateDatabase()
    {
        if (_database != null)
            return _database;

        var properties = _options.ThroughputMode switch
        {
            ThroughputMode.None => null,
            ThroughputMode.Manual => ThroughputProperties.CreateManualThroughput(_options.Throughput),
            ThroughputMode.Autoscale => ThroughputProperties.CreateAutoscaleThroughput(_options.Throughput),
            _ => throw new ArgumentException("CosmosDb ThroughputMode not valid")
        };

        var response = (properties != null) 
            ? await Client.CreateDatabaseIfNotExistsAsync(_options.Database, properties)
            : await Client.CreateDatabaseIfNotExistsAsync(_options.Database);

        if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created)
            throw new SystemException($"Cosmos db {_options.Database} has not been creatred");

        return _database = response.Database;
    }

    protected virtual CosmosClient CreateClient(CosmosDbOptions options)
    {
        return new CosmosClient(options.Endpoint, new DefaultAzureCredential(), new CosmosClientOptions());

        //return new CosmosClient(options.Endpoint, new InteractiveBrowserCredential(), new CosmosClientOptions());
    }
}


