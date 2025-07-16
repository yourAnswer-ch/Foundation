using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Foundation.Logging.EventHubLogger.Interface;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Hosting;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Foundation.Logging.EventHubLogger;

internal class MessagePublisher(IAzureClientFactory<EventHubProducerClient> factory, IMessageQueue queue) : BackgroundService
{
    private const int MaxBlockSize = 20;
    private readonly EventHubProducerClient _client = factory.CreateClient("EventHubLogger");

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                var block = queue.Messages.Count;

                if (block == 0)
                {
                    var item = queue.Messages.Take(ct);
                    await _client.SendAsync([new EventData(item.Serialize())]);
                }
                else
                {
                    await SendElements(_client, queue.Messages, block, ct);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceWarning(ex.ToString());
            }
        }
    }

    private static async Task SendElements(
        EventHubProducerClient client,
        BlockingCollection<LogEntry> entries,
        int elements, CancellationToken token)
    {
        var count = elements <= MaxBlockSize ? elements : MaxBlockSize;
        var list = new List<EventData>(count);

        for (var i = 0; i < count; i++)
        {
            var message = entries.Take(token);
            list.Add(new EventData(message.Serialize()));
        }

        await client.SendAsync(list, token);
    }
}