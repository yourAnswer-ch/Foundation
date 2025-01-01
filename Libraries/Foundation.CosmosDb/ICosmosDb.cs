using Microsoft.Azure.Cosmos;

namespace Foundation.CosmosDb;

public interface ICosmosDb
{
    bool UseCamelCase { get; }

    Task<Database> GetOrCreateDatabase();
}

