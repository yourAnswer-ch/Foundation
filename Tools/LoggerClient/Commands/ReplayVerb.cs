using CommandLine;

namespace CloudLogger.Commands;

[Verb("replay", HelpText = "replays messages")]
class ReplayVerb : IAction
{
    //[Value(0, HelpText = "Timespan String hh:mm:ss")]
    //public string? Time { get; set; }

    public void Action()
    {
        try
        {
            Program.host?.Rewind(TimeSpan.Zero).Wait();

            //if (TimeSpan.TryParseExact(Time, @"hh\:mm\:ss", null, out TimeSpan t))
            //{
            //    Program.host?.Rewind(t).Wait();
            //}
            //else
            //{
            //    Console.WriteLine("Can't parse Timespan, must be in Format 'hh:mm:ss'");
            //}
        }
        catch (Exception e)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"An error occured while trying to connect to environment;");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(e.Message);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
