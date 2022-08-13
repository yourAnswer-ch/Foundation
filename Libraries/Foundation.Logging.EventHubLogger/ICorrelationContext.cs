
namespace Foundation.Logging.EventHubLogger;

public interface ICorrelationContext
{
    string CorrelationId { get; }
}
