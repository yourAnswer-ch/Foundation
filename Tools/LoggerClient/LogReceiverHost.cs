using Azure.Messaging.EventHubs.Consumer;
using CloudLogger.Filtering;
using Microsoft.Azure.Amqp.Framing;

namespace CloudLogger;

internal class LogReceiverHost(Columns columns, LogFilter filter, LogWriter writer)
{
    public EventHubConnection? Connection { get; private set; }

    private readonly List<LogReceiver> _receivers = [];
    private static readonly SemaphoreSlim Mutex = new(1);

    public DateTime LastUpdate => _receivers.Any() ? _receivers.Max(e => e.LastUpdate) : DateTime.Now;

    public void Suspend(bool status)
    {
        try
        {
            Mutex.WaitAsync();
            _receivers.ForEach(e => e.Suspend(status));
        }
        finally
        {
            Mutex.Release();
        }
    }

    public void Stop()
    {
        try
        {
            _receivers.ForEach(e => e.Stop());
            _receivers.Clear();
        }
        finally
        {
            Mutex.Release();
        }
    }

    public async Task Connect(EventHubConnection connection, bool startWithEarliestEvent) //DateTime startDateTime)
    {
        await Mutex.WaitAsync();
        try
        {
            Connection = connection;
            var client = new EventHubConsumerClient(
                EventHubConsumerClient.DefaultConsumerGroupName, 
                connection.ConnectionString);

            var receiver = new LogReceiver(columns, filter, writer);
            //receiver.LaunchProcess(client, EventPosition.FromEnqueuedTime(startDateTime));
            receiver.LaunchProcess(client, startWithEarliestEvent);
            _receivers.Add(receiver);
        }
        finally
        {
            Mutex.Release();
        }
    }

    public async Task Rewind(TimeSpan time)
    {
        Stop();
        await Mutex.WaitAsync();
        try
        {
            var connection = Connection?.ConnectionString;
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            //var factory = EventHubClient.CreateFromConnectionString(conncection);
            var client = new EventHubConsumerClient(
                EventHubConsumerClient.DefaultConsumerGroupName,
            connection);

            var receiver = new LogReceiver(columns, filter, writer);
            receiver.LaunchProcess(client, true);
            _receivers.Add(receiver);
        }
        finally
        {
            Mutex.Release();
        }
    }
}