using System.Diagnostics;
using CloudLogger.Filtering;
using Foundation.Logging.EventHubLogger.Interface;
using Microsoft.Azure.EventHubs;


namespace CloudLogger;

internal class LogReceiver
{
    private readonly LogFilter _filter;
    private readonly LogWriter _writer;
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

    public void LaunchProcess(EventHubClient factory, string partitionId, EventPosition position)
    {
        _source = new CancellationTokenSource();            
        _currenTask = Task.Factory.StartNew(
            () => ProcessMessage(factory, partitionId, position, _source.Token),
            _source.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
    }

    private async Task ProcessMessage(EventHubClient factory, string partitionId, EventPosition position, CancellationToken token)
    {
        try
        {
            var receiver = factory.CreateReceiver("$Default", partitionId, position);
            Trace.WriteLine($"Receiver for partition: '{partitionId}' created.");

            try
            {
                while (!token.IsCancellationRequested)
                {
                    var messages = await receiver.ReceiveAsync(100);

                    if (messages == null)
                        continue;

                    foreach (var message in messages)
                    {
                        if(message.Body.Array == null)
                            continue;

                        var body = new MemoryStream(message.Body.Array);
                        var entry = body.Deserialize();

                        _block.WaitOne();

                        LastUpdate = entry.Timestamp;

                        if (_filter.Match(entry) && !token.IsCancellationRequested)
                            _writer.WriteMessage(entry);
                    }
                }
            }
            finally
            {
                await receiver.CloseAsync();
                Trace.WriteLine($"Receiver for partition: '{partitionId}' closed.");
            }
        }
        catch(QuotaExceededException)
        {
            ReportException($"Exceeded the maximum number of allowed receivers per partition in a consumer group which is 5.");
        }
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

    public LogReceiver(LogFilter filter, LogWriter writer)
    {
        _filter = filter;
        _writer = writer;
    }
}