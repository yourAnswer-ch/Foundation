using Microsoft.Azure.Cosmos;

namespace Foundation.CosmosDb;

public static class UpsertItemExtension
{
    public static async Task<T> UpsertItemAsync<T>(this ICosmosDbContainer container, T item, string partitionKey)
    {
        return await container.UpsertItemAsync(item, new PartitionKey(partitionKey));
    }

    public static async Task<T> UpsertItemAsync<T>(this ICosmosDbContainer container, T item, PartitionKey? partitionKey = null)
    {
        var current = await container.GetOrCreateContainer();

        var response = await current.UpsertItemAsync(item, partitionKey);

        return response.Resource;
    }

    public static async Task<T?> DeleteItemAsync<T>(this ICosmosDbContainer container, string id, string partitionKey)
    {
        var current = await container.GetOrCreateContainer();

        var result = await current.DeleteItemAsync<T>(id, new PartitionKey(partitionKey));

        return result.Resource;
    }
}
