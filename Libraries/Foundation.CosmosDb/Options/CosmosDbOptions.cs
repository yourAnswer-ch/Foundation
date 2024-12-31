using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace Foundation.CosmosDb.Options;

public enum ThroughputMode
{
    None, Manual, Autoscale
}

[OptionsValidator]
public sealed partial class CosmosDbOptions : IValidateOptions<CosmosDbOptions>
{
    [Url, Required(AllowEmptyStrings = false)]
    public string Endpoint { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string Database { get; set; }

    public ThroughputMode ThroughputMode { get; set; } = ThroughputMode.None;

    public int Throughput { get; set; } = 400;

    public string Region { get; set; }

    public string PrimaryKey { get; set; }

    // Track all container registrations
    public List<ContainerOptions> Containers { get; set; } = new();
}

