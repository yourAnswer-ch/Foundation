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
    public required string Endpoint { get; set; }

    [Required(AllowEmptyStrings = false)]
    public required string Database { get; set; }

    public ThroughputMode ThroughputMode { get; set; } = ThroughputMode.None;

    public int Throughput { get; set; } = 400;

    public string? Region { get; set; }

    public string? Key { get; set; }
}

