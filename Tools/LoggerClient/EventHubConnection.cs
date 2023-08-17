using Azure.Messaging.EventHubs.Producer;

namespace CloudLogger;

internal class EventHubConnection
{
    public string Host { get; }

    public string ConnectionString { get; }

    public string EntryPath { get; }

    public EventHubConnection(string connectionString)
    {
        var builder = new EventHubProducerClient(connectionString);
        Host = builder.EventHubName; //  .Endpoint.Host;
        ConnectionString = connectionString;
        EntryPath = builder.FullyQualifiedNamespace; // .EntityPath;
    }
}