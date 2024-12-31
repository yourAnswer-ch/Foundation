using Microsoft.Azure.Cosmos;

namespace Foundation.CosmosDb;

public abstract class CosmosContainer : ICosmosDbContainer
{
    private Container? _container;
    private readonly ICosmosDb _db;
    
    protected CosmosContainer(ICosmosDb db)
    {
        _db = db;
    }

    protected abstract ContainerProperties CreateContainerProperties();

    protected virtual ThroughputProperties? CreateThroughputProperties()
    {
        return null;
    }

    public async Task<Container> CreateIfNotExist()
    {
        if (_container != null)
            return _container;

        var db = await _db.GetOrCreateDatabase();

        var throughput = CreateThroughputProperties();
        var properties = CreateContainerProperties();

        var response = (throughput != null) 
            ? await db.CreateContainerIfNotExistsAsync(properties) 
            : await db.CreateContainerIfNotExistsAsync(properties, throughput);

        return _container = response.Container;
    }
}


