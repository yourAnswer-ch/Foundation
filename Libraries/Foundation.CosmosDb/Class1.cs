using Foundation.CosmosDb.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;


namespace Foundation.CosmosDb;

public class CosmosContainerFactory
{
    private readonly CosmosClient _cosmosClient;
    private readonly CosmosDbOptions _options;

    // Thread-safe cache of container references
    private readonly ConcurrentDictionary<string, Container> _cachedContainers = new();

    public CosmosContainerFactory(
        CosmosClient cosmosClient,
        IOptions<CosmosDbOptions> options)
    {
        _cosmosClient = cosmosClient;
        _options = options.Value;
    }

    /// <summary>
    /// Returns a container for the given name.
    /// Creates it if it doesn’t yet exist in memory or in Cosmos DB.
    /// </summary>
    public async Task<Container> GetContainerAsync(string containerName)
    {
        // Return from cache if already created
        if (_cachedContainers.TryGetValue(containerName, out var cached))
        {
            return cached;
        }

        // Otherwise, create the container in Cosmos if needed
        var containerConfig = _options.Containers
            .FirstOrDefault(x => x.ContainerName == containerName);

        if (containerConfig == null)
        {
            throw new InvalidOperationException(
                $"No container configuration found for container '{containerName}'."
            );
        }

        // Create the database if not exists
        var databaseResponse = await _cosmosClient.CreateDatabaseIfNotExistsAsync(_options.Database);

        // Create container if not exists
        var containerResponse = await databaseResponse.Database
            .CreateContainerIfNotExistsAsync(containerConfig.ContainerName, containerConfig.PartitionKeyPath);

        // Cache the Container reference
        _cachedContainers[containerName] = containerResponse.Container;

        return containerResponse.Container;
    }

    /// <summary>
    /// Optionally: eagerly create *all* containers in the config
    /// (useful at application startup so repos don’t have to lazily create).
    /// </summary>
    public async Task InitializeAllContainersAsync()
    {
        var databaseResponse = await _cosmosClient.CreateDatabaseIfNotExistsAsync(_options.Database);

        foreach (var containerConfig in _options.Containers)
        {
            // Ensure each container is created
            var containerResponse = await databaseResponse.Database
                .CreateContainerIfNotExistsAsync(containerConfig.ContainerName, containerConfig.PartitionKeyPath);

            // Update the dictionary cache
            _cachedContainers[containerConfig.ContainerName] = containerResponse.Container;
        }
    }
}

public interface IFluentCosmosBuilder
{
    /// <summary>
    /// Registers a container with a specified name and partition key path.
    /// Also registers the repository type TRepository in DI.
    /// </summary>
    IFluentCosmosBuilder AddContainer<TRepository>(
        string containerName,
        string partitionKeyPath
    ) where TRepository : class;

    /// <summary>
    /// Finalizes the registrations in DI.
    /// </summary>
    IServiceCollection Build();
}

internal class FluentCosmosBuilder : IFluentCosmosBuilder
{
    private readonly IServiceCollection _services;
    private readonly CosmosDbOptions _options;

    public FluentCosmosBuilder(IServiceCollection services, CosmosDbOptions options)
    {
        _services = services;
        _options = options;
    }

    public IFluentCosmosBuilder AddContainer<TRepository>(
        string containerName,
        string partitionKeyPath
    ) where TRepository : class
    {
        // Store the container configuration
        _options.Containers.Add(new ContainerOptions
        {
            ContainerName = containerName,
            PartitionKeyPath = partitionKeyPath,
            RepositoryType = typeof(TRepository)
        });

        // Register the repository
        _services.AddScoped<TRepository>();

        return this;
    }

    public IServiceCollection Build()
    {
        // If there are final steps to do with the collected container registrations, do them here
        // Otherwise, just return the services
        return _services;
    }
}



public abstract class CosmosRepositoryBase
{
    protected readonly CosmosClient CosmosClient;
    protected readonly IOptions<CosmosDbOptions> CosmosDbOptions;

    protected Container _container; // Lazy backing field

    protected abstract string ContainerName { get; }

    protected abstract string PartitionKeyPath { get; }

    protected CosmosRepositoryBase(
        CosmosClient cosmosClient,
        IOptions<CosmosDbOptions> cosmosDbOptions)
    {
        CosmosClient = cosmosClient;
        CosmosDbOptions = cosmosDbOptions;
    }

    /// <summary>
    /// Lazy-load or create the container if it doesn't exist. 
    /// This will be called before each operation, or you can call it once in a separate initialization flow.
    /// </summary>
    protected async Task<Container> GetContainerAsync()
    {
        if (_container != null)
            return _container;

        var dbName = CosmosDbOptions.Value.Database;
        // You can either do 'create if not exists' here, or rely on external init
        var database = await CosmosClient.CreateDatabaseIfNotExistsAsync(dbName);

        // create container if not exists
        _container = await database.Database.CreateContainerIfNotExistsAsync(
            ContainerName,
            PartitionKeyPath
        );

        return _container;
    }
}



internal class Class1
{
}
