using System.Collections.Concurrent;
using System.Diagnostics;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Foundation.Logging.EventHubLogger.Interface;

namespace Foundation.Logging.EventHubLogger;

internal class MessagePump : IMessagePump
{
    private Task? _worker;
    private EventHubProducerClient _client;
    private readonly BlockingCollection<LogEntry> _messages;

    public  void Append(LogEntry logEntry)
    {
        _messages.Add(logEntry);
    }

    public  void Start()
    {           
        if (_worker != null && !(_worker.IsCanceled || _worker.IsCompleted || _worker.IsFaulted))
            return;
       
        _worker = Task.Factory.StartNew(async () =>
        {
            while (true)
            {
                try
                {
                    var block = _messages.Count;

                    if (block == 0)
                    {
                        var item = _messages.Take();
                        await _client.SendAsync(new EventData[] { new EventData(item.Serialize()) });
                    }
                    else
                    {
                        var list = new List<EventData>();
                        var stop = block <= 20 ? block : 20;
                        
                        for (var i = 0; i < stop; i++)
                        {
                            var message = _messages.Take();
                            list.Add(new EventData(message.Serialize()));
                        }

                        await _client.SendAsync(list);
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceWarning(ex.ToString());
                }
            }
        }, TaskCreationOptions.LongRunning);
    }

    public MessagePump(EventHubLoggerOptions options)
    {
        _client = new EventHubProducerClient(options.ConnectionString);
        _messages = new BlockingCollection<LogEntry>();
    }
}
