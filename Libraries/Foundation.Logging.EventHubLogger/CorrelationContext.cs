
namespace Foundation.Logging.EventHubLogger;

internal class CorrelationContext(string correlationId) : ICorrelationContext
{
    public string CorrelationId { get; } = correlationId;
}
