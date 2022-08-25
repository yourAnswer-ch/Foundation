using System.Collections.Concurrent;
using System.Diagnostics;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Foundation.Logging.EventHubLogger.Interface;

namespace Foundation.Logging.EventHubLogger;

internal class MessagePump : IMessagePump
{
    private Task? _worker;
    private const int MaxBlockSize = 20;
    
    private CancellationTokenSource _source;
    private readonly EventHubProducerClient _client;    
    private readonly BlockingCollection<LogEntry> _messages;    

    public  void Append(LogEntry logEntry)
    {
        _messages.Add(logEntry);
    }

    public void Start()
    {        
        if (_worker != null && !(_worker.IsCanceled || _worker.IsCompleted || _worker.IsFaulted))
            return;
        
        _source = new CancellationTokenSource();        
        _worker = Task.Factory.StartNew(async () =>
        {
            while (!_source.Token.IsCancellationRequested)
            {
                try
                {
                    var block = _messages.Count;

                    if (block == 0)
                    {
                        var item = _messages.Take(_source.Token);
                        await _client.SendAsync(new EventData[] { new EventData(item.Serialize()) });
                    }
                    else
                    {
                        await SendElements(_client, _messages, block, _source.Token);
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceWarning(ex.ToString());
                }
            }
        }, TaskCreationOptions.LongRunning);
    }

    public void Stop()
    {
        _source.Cancel(false);
        while(_messages.Count > 0)
        {
            SendElements(_client, _messages, _messages.Count, CancellationToken.None).RunSynchronously(); 
        }
    }

    private async Task SendElements(EventHubProducerClient client, BlockingCollection<LogEntry> entries, int elements, CancellationToken token)
    {        
        var count = elements <= MaxBlockSize ? elements : MaxBlockSize;
        var list = new List<EventData>(count);

        for (var i = 0; i < count; i++)
        {
            var message = entries.Take(token);
            list.Add(new EventData(message.Serialize()));
        }

        await client.SendAsync(list);
    }

    public MessagePump(EventHubLoggerOptions options)
    {
        _source = new CancellationTokenSource();
        _client = new EventHubProducerClient(options.ConnectionString);
        _messages = new BlockingCollection<LogEntry>();
    }
}
