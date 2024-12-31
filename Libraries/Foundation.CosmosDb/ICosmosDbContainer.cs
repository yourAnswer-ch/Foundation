using Microsoft.Azure.Cosmos;

namespace Foundation.CosmosDb;

public interface ICosmosDbContainer
{
    Task<Container> CreateIfNotExist();
}


