using Microsoft.Azure.EventHubs;

namespace CloudLogger;

internal class EventHubConnection
{
    public string Host { get; }

    public string ConnectionString { get; }

    public string EntryPath { get; }

    public EventHubConnection(string connectionString)
    {
        var builder = new EventHubsConnectionStringBuilder(connectionString);
        Host = builder.Endpoint.Host;
        ConnectionString = connectionString;
        EntryPath = builder.EntityPath;
    }
}