using Microsoft.Azure.Cosmos;

namespace Foundation.CosmosDb;

public static class GetItemExtensions
{
    public static async Task<T?> GetItemByIdAsync<T>(
        this ICosmosDbContainer container,
        string id,
        string partitionKey)
    {
        var current = await container.GetOrCreateContainer();

        try { 
            var result = await current.ReadItemAsync<T>(id, new PartitionKey(partitionKey));
            return result.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return default;
        }
    }

    public static async Task<T?> GetItemAsync<T>(
        this ICosmosDbContainer container,
        string query,
        object parameters)
    {
        return await container.GetItemAsync<T>(query.CreateQueryFromObject(parameters, container.UseCamelCase));
    }

    public static async Task<T?> GetItemAsync<T>(
        this ICosmosDbContainer container,
        string query,
        object parameters,
        string partitionKey)
    {
        return await container.GetItemAsync<T>(
            query.CreateQueryFromObject(parameters, container.UseCamelCase),
            queryRequestOptions: new QueryRequestOptions
            {
                PartitionKey = new PartitionKey(partitionKey)
            });
    }

    public static async Task<T?> GetItemAsync<T>(
        this ICosmosDbContainer container,
        string query,
        IDictionary<string, object> parameters)
    {
        return await container.GetItemAsync<T>(query.CreateQuery(parameters, container.UseCamelCase));
    }

    public static async Task<T?> GetItemAsync<T>(
        this ICosmosDbContainer container,
        string query,
        IDictionary<string, object> parameters,
        string partitionKey)
    {
        return await container.GetItemAsync<T>(
            query.CreateQueryFromObject(parameters, container.UseCamelCase),
            queryRequestOptions: new QueryRequestOptions
            {
                PartitionKey = new PartitionKey(partitionKey)
            });
    }

    public static async Task<T?> GetItemAsync<T>(
    this ICosmosDbContainer container,
    QueryDefinition queryDefinition,
    string? continuationToken = null,
    QueryRequestOptions? queryRequestOptions = null)
    {
        var current = await container.GetOrCreateContainer();

        var result = current.GetItemQueryIterator<T>(
            queryDefinition,
            continuationToken,
            queryRequestOptions);

        if (result == null || !result.HasMoreResults)
            return default;

        var response = await result.ReadNextAsync();
        return response.Resource.SingleOrDefault();
    }
}