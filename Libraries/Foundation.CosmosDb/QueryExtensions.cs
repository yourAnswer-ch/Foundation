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
        return container.QueryItemsAsync<T>(CreateQuery(query, parameters));
    }

    public static IAsyncEnumerable<T> QueryItemsAsync<T>(
        this ICosmosDbContainer container,
        string query,
        IDictionary<string, object> parameters,
        string partitionKey)
    {
        return container.QueryItemsAsync<T>(
            CreateQuery(query, parameters),
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
        return container.QueryItemsAsync<T>(CreateQueryFromObject(query, parameters));
    }

    public static IAsyncEnumerable<T> QueryItemsAsync<T>(
        this ICosmosDbContainer container,
        string query,
        object parameters,
        string partitionKey)
    {
        return container.QueryItemsAsync<T>(
            CreateQueryFromObject(query, parameters),
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

    #region internal functions
    private static QueryDefinition CreateQuery(string query, IDictionary<string, object> parameters)
    {
        var queryDefinition = new QueryDefinition(query);
        foreach (var parameter in parameters)
        {
            var key = parameter.Key.StartsWith('@') ? parameter.Key : $"@{parameter.Key}";
            queryDefinition.WithParameter(key, parameter.Value);
        }

        return queryDefinition;
    }

    private static QueryDefinition CreateQueryFromObject(string query, object parameters)
    {
        var queryDefinition = new QueryDefinition(query);

        if (parameters == null)
            return queryDefinition;

        var type = parameters.GetType();
        var properties = type.GetProperties();

        foreach (var property in properties)
        {
            var value = property.GetValue(parameters);
            queryDefinition.WithParameter($"@{property.Name}", value);
        }

        return queryDefinition;
    }
    #endregion
}
