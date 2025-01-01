using Microsoft.Azure.Cosmos;

namespace Foundation.CosmosDb;

public abstract class CosmosContainer(ICosmosDb database) : ICosmosDbContainer
{
    private Container? _container;

    public bool UseCamelCase => database.UseCamelCase;

    protected abstract ContainerProperties CreateContainerProperties();

    protected virtual ThroughputProperties? CreateThroughputProperties()
    {
        return null;
    }

    public async Task<Container> GetOrCreateContainer()
    {
        if (_container != null)
            return _container;

        var db = await database.GetOrCreateDatabase();

        var throughput = CreateThroughputProperties();
        var properties = CreateContainerProperties();

        var response = await db.CreateContainerIfNotExistsAsync(properties, throughput);

        return _container = response.Container;
    }
}


