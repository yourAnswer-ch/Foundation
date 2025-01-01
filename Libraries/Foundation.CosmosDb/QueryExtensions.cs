using Microsoft.Azure.Cosmos;

namespace Foundation.CosmosDb;

public static class QueryExtensions
{
    #region Query without parameters
    public static IAsyncEnumerable<T> QueryItemsAsync<T>(
        this ICosmosDbContainer container,
        string query)
    {
        return container.QueryItemsAsync<T>(new QueryDefinition(query));
    }

    public static IAsyncEnumerable<T> QueryItemsAsync<T>(this ICosmosDbContainer container, string query, string partitionKey)
    {
        return container.QueryItemsAsync<T>(
            new QueryDefinition(query),
            queryRequestOptions: new QueryRequestOptions
            {
                PartitionKey = new PartitionKey(partitionKey)
            });
    }

    #endregion

    #region Query with parameters in dictionary

    public static IAsyncEnumerable<T> QueryItemsAsync<T>(
        this ICosmosDbContainer container,
        string query,
        IDictionary<string, object> parameters)
    {
        return container.QueryItemsAsync<T>(query.CreateQuery(parameters, container.UseCamelCase));
    }

    public static IAsyncEnumerable<T> QueryItemsAsync<T>(
        this ICosmosDbContainer container,
        string query,
        IDictionary<string, object> parameters,
        string partitionKey)
    {
        return container.QueryItemsAsync<T>(
            query.CreateQuery(parameters, container.UseCamelCase),
            queryRequestOptions: new QueryRequestOptions
            {
                PartitionKey = new PartitionKey(partitionKey)
            });
    }

    #endregion

    #region Query with paramters as anonym object

    public static IAsyncEnumerable<T> QueryItemsAsync<T>(
        this ICosmosDbContainer container,
        string query,
        object parameters)
    {
        return container.QueryItemsAsync<T>(query.CreateQueryFromObject(parameters, container.UseCamelCase));
    }

    public static IAsyncEnumerable<T> QueryItemsAsync<T>(
        this ICosmosDbContainer container,
        string query,
        object parameters,
        string partitionKey)
    {
        return container.QueryItemsAsync<T>(
            query.CreateQueryFromObject(parameters, container.UseCamelCase),
            queryRequestOptions: new QueryRequestOptions
            {
                PartitionKey = new PartitionKey(partitionKey)
            });
    }

    #endregion

    #region Query with native objects

    public static async IAsyncEnumerable<T> QueryItemsAsync<T>(
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

        while (result != null && result.HasMoreResults)
        {
            foreach (T document in await result.ReadNextAsync())
            {
                yield return document;
            }
        }
    }
    #endregion
}
