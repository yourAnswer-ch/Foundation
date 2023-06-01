using Foundation.Logging.EventHubLogger.Interface;

namespace CloudLogger;

internal class LogWriter
{
    internal static object Mutex = new object();

    public virtual void WriteMessage(LogEntry logEntry)
    {
        var message = logEntry.ToString(Program.showName);
        var color = GetColor((int) logEntry.LogLevel);
        lock (Console.Out)
        {
            if (Console.ForegroundColor == color)
            {
                Console.WriteLine(message);
            }
            else
            {
                Console.ForegroundColor = color;
                Console.WriteLine(message);
            }
        }
    }

    private static ConsoleColor GetColor(int logLevel)
    {
        return logLevel switch
        {
            // Trace,
            0 => ConsoleColor.Gray,
            //Debug,
            1 => ConsoleColor.White,
            //Information,
            2 => ConsoleColor.Green,
            //Warning,
            3 => ConsoleColor.Yellow,
            //Error,
            4 => ConsoleColor.Red,
            //Critical,
            5 => ConsoleColor.DarkRed,
            //None,
            6 => ConsoleColor.Blue,
            _ => ConsoleColor.Green,
        };
    }
}