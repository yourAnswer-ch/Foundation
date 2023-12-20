using Foundation.Logging.EventHubLogger.Interface;

namespace CloudLogger;

internal class LogWriter
{
    internal static object Mutex = new object();

    public virtual void WriteMessage(Columns columns, LogEntry log)
    {
        var color = GetColor((int)log.LogLevel);
        lock (Console.Out)
        {
            if (Console.ForegroundColor != color)
                Console.ForegroundColor = color;

            if (columns.ShowTimestamp)
            {
                Console.Out.Write("[");
                Console.Out.Write(log.Timestamp.ToString("HH:mm:ss.ffff ddMMyy"));
                Console.Out.Write("] ");
            }

            if (columns.ShowLogLevel)
            {
                Console.Out.Write(log.LogLevel.ToString().PadRight(11));
                Console.Out.Write(" ");
            }

            if (columns.ShowHost)
            {
                Console.Out.Write("[");
                Console.Out.Write(log.Host);
                Console.Out.Write("] ");
            }

            if (columns.ShowApp)
            {
                Console.Out.Write("[");
                Console.Out.Write(log.App);
                Console.Out.Write("] ");
            }

            if (columns.ShowName)
            {
                Console.Out.Write("[");
                Console.Out.Write(log.Name);
                Console.Out.Write("] ");
            }

            if (columns.ShowCorrelationId && !string.IsNullOrEmpty(log.CorrelationId))
            {
                Console.Out.Write("[");
                Console.Out.Write(log.CorrelationId);
                Console.Out.Write("] ");
            }

            if (columns.ShowMessage)
            {
                Console.Out.Write(log.Message);
            }

            Console.Out.WriteLine();
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
