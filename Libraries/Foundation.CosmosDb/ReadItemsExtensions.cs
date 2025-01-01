using Microsoft.Azure.Cosmos;

namespace Foundation.CosmosDb;

public static class GetItemExtensions
{
    public static async Task<T> GetItemAsync<T>(this ICosmosDbContainer container, string id, string partitionKey)
    {
        var current = await container.GetOrCreateContainer();
        
        var result = await current.ReadItemAsync<T>(id, new PartitionKey(partitionKey));

        return result.Resource;
    }

    //public static async Task<T> GetItemAsync<T>(
    //this ICosmosDbContainer container,
    //QueryDefinition queryDefinition,
    //string? continuationToken = null,
    //QueryRequestOptions? queryRequestOptions = null)
    //{
    //    var current = await container.GetOrCreateContainer();

    //    var result = current.GetItemQueryIterator<T>(
    //        queryDefinition,
    //        continuationToken,
    //        queryRequestOptions);

    //    while (result != null && result.HasMoreResults)
    //    {
    //        foreach (T document in await result.ReadNextAsync())
    //        {
    //            yield return document;
    //        }
    //    }
    //}
}