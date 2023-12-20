using System.Diagnostics;
using CloudLogger.Filtering;
using Foundation.Logging.EventHubLogger.Interface;
using Azure.Messaging.EventHubs.Consumer;

namespace CloudLogger;

internal class LogReceiver(Columns columns, LogFilter filter, LogWriter writer)
{
    private readonly ManualResetEvent _block = new ManualResetEvent(true);

    private Task? _currenTask;
    private CancellationTokenSource? _source;
    
    public DateTime LastUpdate { get; private set; }

    public void Suspend(bool status)
    {
        if (status)
            _block.Reset();
        else
            _block.Set();
    }

    public void Stop()
    {
        _source?.Cancel();
        _block.Set();

        _currenTask?.Wait();
    }

    public bool IsRunning => _source?.IsCancellationRequested ?? false;       

    public void LaunchProcess(EventHubConsumerClient factory, bool startWithEarliestEvent)
    {
        _source = new CancellationTokenSource();            
        _currenTask = Task.Factory.StartNew(
            () => ProcessMessage(factory, startWithEarliestEvent, _source.Token),
            _source.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
    }

    private async Task ProcessMessage(EventHubConsumerClient client, bool startWithEarliestEvent, CancellationToken token)
    {
        try
        {
            Trace.WriteLine($"Receiver created.");
           
            try
            {
                while (!token.IsCancellationRequested)
                {
                    await foreach (var messages in client.ReadEventsAsync(startWithEarliestEvent, cancellationToken: token))
                    {
                        var stream = messages.Data.EventBody.ToStream();
                        var entry = stream.Deserialize();

                        LastUpdate = entry.Timestamp;

                        if (filter.Match(entry) && !token.IsCancellationRequested)
                            writer.WriteMessage(columns, entry);
                    }
                }
            }
            finally
            {
                await client.CloseAsync();
                Trace.WriteLine($"Receiver closed.");
            }
        }
        //catch (QuotaExceededException)
        //{
        //    ReportException($"Exceeded the maximum number of allowed receivers per partition in a consumer group which is 5.");
        //}
        catch (Exception ex)
        {
            ReportException($"Receiver stoped: {ex.Message}");
        }
    }

    private void ReportException(string message)
    {
        Trace.WriteLine(message);

        //if (!_source.IsCancellationRequested)
        //{
            lock (Console.Out)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine(message);
                Console.ForegroundColor = ConsoleColor.White;
            }
            _source?.Cancel();
        //}
    }
}