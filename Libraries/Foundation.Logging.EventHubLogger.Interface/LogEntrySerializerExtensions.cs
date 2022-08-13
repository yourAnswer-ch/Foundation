using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Foundation.Logging.EventHubLogger.Interface;

public static class LogEntrySerializerExtensions
{
    public static byte[] Serialize(this LogEntry entry)
    {
        var stream = new MemoryStream();
        Serialize(entry, stream);
        return stream.ToArray();
    }

    public static void Serialize(this LogEntry entry, Stream stream)
    {
        var writer = new BinaryWriter(stream);
        writer.Write(JsonConvert.SerializeObject(entry));
    }

    public static LogEntry Deserialize(this Stream stream)
    {
        var reader = new BinaryReader(stream);

        var entry = JsonConvert.DeserializeObject<LogEntry>(reader.ReadString());

        if (entry == null) //fallback for old binary format --> replace after migration
        {
            stream.Position = 0;
            entry = new LogEntry
            {
                LogLevel = (LogLevel)reader.ReadInt32(),
                EventId = reader.ReadInt32(),
                EventName = reader.ReadString(),
                Host = reader.ReadString(),
                App = reader.ReadString(),
                Name = reader.ReadString(),
                Message = reader.ReadString(),
                Timestamp = DateTime.FromBinary(reader.ReadInt64()),
            };
        }

        return entry;
    }
}