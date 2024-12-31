using Microsoft.Azure.Cosmos;

namespace Foundation.CosmosDb;

public interface ICosmosDb
{
    Task<Database> GetOrCreateDatabase();
}

