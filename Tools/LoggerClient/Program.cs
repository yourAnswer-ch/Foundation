using CloudLogger.Commands;
using CloudLogger.Filtering;


namespace CloudLogger;

public class Program
{
    internal static bool leave;
    internal static bool showName = false;
    internal static Columns columns = new Columns();
    internal static LogFilter? filter;
    internal static LogReceiverHost? host;
    internal static FileLogWriter? writer;    
    internal static Timer? headerUpdateTime;

    static void Main(string[] args)
    {          
        writer = new FileLogWriter(@"spool.log");

        filter = new LogFilter();
        host = new LogReceiverHost(columns, filter, writer);

        CommandVerbs.ParseStart(args);

        headerUpdateTime = new Timer(s => SetHeadline(), null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));

        while (!leave)
        {
            if (Console.KeyAvailable)
            {
                host.Suspend(true);

                Console.ForegroundColor = ConsoleColor.White;
                var value = Console.ReadLine();

                if (value != null)
                {
                    CommandVerbs.Parse(value);
                }

                host.Suspend(false);
            }

            Thread.Sleep(200);
        }

        Console.WriteLine("Shutdown lisener tasks...");
        host.Stop();
    }
 
    internal static void SetHeadline()
    {
        Console.Title = $"Host: '{host?.Connection?.Host}' - Filter: '{filter}' - LastUpdate: {host?.LastUpdate}";
    }
}