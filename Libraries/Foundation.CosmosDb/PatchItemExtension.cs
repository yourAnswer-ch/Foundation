using Microsoft.Azure.Cosmos;

namespace Foundation.CosmosDb;

public static class PatchItemExtension
{
    public static async Task<T?> PatchItemAsync<T>(
    this ICosmosDbContainer container,
    string id,
    string partitionKey,
    IReadOnlyList<PatchOperation> patches)
    {
        return await container.PatchItemAsync<T>(id, new PartitionKey(partitionKey), patches);
    }

    public static async Task<T?> PatchItemAsync<T>(
        this ICosmosDbContainer container, 
        string id,
        PartitionKey partitionKey, 
        IReadOnlyList<PatchOperation> patches)
    {
        var current = await container.GetOrCreateContainer();

        var result = await current.PatchItemAsync<T>(id, partitionKey, patches);

        return result.Resource;
    }
}