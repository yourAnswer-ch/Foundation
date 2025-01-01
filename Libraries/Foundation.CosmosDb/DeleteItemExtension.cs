using Microsoft.Azure.Cosmos;

namespace Foundation.CosmosDb;

public static class DeleteItemExtension
{
    public static async Task<T?> DeleteItemAsync<T>(this ICosmosDbContainer container, string id, string partitionKey)
    {
        return await container.DeleteItemAsync<T>(id, new PartitionKey(partitionKey));
    }

    public static async Task<T?> DeleteItemAsync<T>(this ICosmosDbContainer container, string id, PartitionKey partitionKey)
    {
        var current = await container.GetOrCreateContainer();

        var result = await current.DeleteItemAsync<T>(id, partitionKey);

        return result.Resource;
    }
}
