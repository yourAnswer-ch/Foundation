
namespace Foundation.Logging.EventHubLogger;

internal class CorrelationContext : ICorrelationContext
{
    public CorrelationContext(string correlationId)
    {
        CorrelationId = correlationId;
    }

    public string CorrelationId { get; }
}
