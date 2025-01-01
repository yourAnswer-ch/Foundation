using Microsoft.Azure.Cosmos;

namespace Foundation.CosmosDb;

public interface ICosmosDbContainer
{
    bool UseCamelCase { get; }

    Task<Container> GetOrCreateContainer();
}


