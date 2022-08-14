using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Foundation.Processing.StorageQueue;

public class QueueProcessorService : BackgroundService
{
    private readonly ILogger _log;
    private readonly QueueReciver _queueReciver;

    public QueueProcessorService(QueueReciver queueReciver, ILogger<QueueProcessorService> log)
    {
        _log = log;
        _queueReciver = queueReciver;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _log.LogInformation("Starting send email Message queue processor");

            await _queueReciver.StartReceiveAsync(stoppingToken);
        }
        finally
        {
            _log.LogWarning("Exiting send email Message queue processor");
        }
    }
}