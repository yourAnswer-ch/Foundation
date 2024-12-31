using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Configuration;

namespace Foundation.CosmosDb.Options;

public class ContainerOptions
{
    /// <summary>
    /// The container name, e.g. 'Users'
    /// </summary>
    public string ContainerName { get; set; }

    /// <summary>
    /// Partition key path, e.g. '/userId'
    /// </summary>
    public string PartitionKeyPath { get; set; }

    /// <summary>
    /// A reference type or the actual repository type if needed.
    /// Not strictly necessary, but can be helpful if you want to look them up.
    /// </summary>
    public Type RepositoryType { get; set; }
}

