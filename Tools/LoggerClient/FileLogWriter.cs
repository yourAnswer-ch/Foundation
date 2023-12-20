using Foundation.Logging.EventHubLogger.Interface;
using System.IO;

namespace CloudLogger;

internal class FileLogWriter : LogWriter
{
    public bool Disabled { get; set; } = true;

    public string FileName { get; set; }

    public override void WriteMessage(Columns columns, LogEntry logEntry)
    {
        base.WriteMessage(columns, logEntry);

        if (Disabled) return;

        var message = logEntry + System.Environment.NewLine;

        File.AppendAllTextAsync(FileName, message).Wait();
    }

    public FileLogWriter(string fileName)
    {
        FileName = fileName;
    }
}