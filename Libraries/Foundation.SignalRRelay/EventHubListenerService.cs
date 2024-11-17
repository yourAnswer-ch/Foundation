using Azure.Identity;
using Azure.Messaging.EventHubs.Consumer;
using Foundation.SignalRRelay.Configurations;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace Foundation.SignalRRelay;

public class EventHubListenerService : BackgroundService
{
    private readonly ILogger<EventHubListenerService> _log;
    private readonly IHubContext<ObjectUpdateHub> _hubContext;
    private readonly EventHubConsumerClient _eventHubConsumerClient;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public EventHubListenerService(
        IOptions<EventHubOptions> options,
        IHubContext<ObjectUpdateHub> hubContext,
        ILogger<EventHubListenerService> log)
    {
        _log = log;
        _hubContext = hubContext;
        _eventHubConsumerClient = new EventHubConsumerClient(
            EventHubConsumerClient.DefaultConsumerGroupName,
            options.Value.Namespace,
            options.Value.Name,
            new DefaultAzureCredential());

        _cancellationTokenSource = new CancellationTokenSource();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _log.LogInformation("SignalR relay event hub listener started.");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            await MessaginLoop(stoppingToken);
        }
        
        _log.LogInformation("SignalR relay event hub listener exit.");
    }

    private async Task MessaginLoop(CancellationToken stoppingToken)
    {
        try
        {            
            await foreach (PartitionEvent partitionEvent in _eventHubConsumerClient.ReadEventsAsync(_cancellationTokenSource.Token))
            {
                if (!partitionEvent.Data.Properties.ContainsKey("type"))
                {
                    _log.LogWarning("Expected property 'type' missing.");
                    continue;
                }

                var type = partitionEvent.Data.Properties["type"]?.ToString();
                if (string.IsNullOrWhiteSpace(type))
                {
                    _log.LogWarning("Received message without type property.");
                    continue;
                }

                var payload = partitionEvent.Data.EventBody.ToString();
                if (string.IsNullOrWhiteSpace(payload))
                {
                    _log.LogWarning("Received message without payload.");
                    continue;
                }

                _log.LogInformation("Forward Message - Id: {0}", partitionEvent.Data.CorrelationId);
                await _hubContext.Clients.All.SendAsync("Update", type, payload);
            }
        }
        catch (Exception ex)
        {
            _log.LogError(ex, $"Error receiving Event Hub messages: {0}", ex.Message);
        }
    }
}
