using Microsoft.Azure.Cosmos;


namespace Foundation.CosmosDb;

public static class QueryExtensions
{
    //public static async IAsyncEnumerable<T> GetDocuments<T>(this ICosmosDbContainer container, string query, CancellationToken cancellationToken = default)
    //{
    //    var current = await container.GetOrCreateContainer();

    //    current.GetItemQueryIterator<T>(query);
    //}

    public static async IAsyncEnumerable<T> QueryAsync<T>(this ICosmosDbContainer container, QueryDefinition query, string? partitionKey = null)
    {
        var current = await container.GetOrCreateContainer();
        var options = partitionKey != null ? new QueryRequestOptions
        {
            PartitionKey = new PartitionKey(partitionKey)
        } : null;

        var result = current.GetItemQueryIterator<T>(query, requestOptions: options);

        while (result != null && result.HasMoreResults)
        {
            foreach (T document in await result.ReadNextAsync())
            {
                yield return document;
            }
        }
    }
}
