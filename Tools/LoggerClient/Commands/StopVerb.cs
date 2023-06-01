using CommandLine;

namespace CloudLogger.Commands;

[Verb("stop", HelpText = "stop receive messages")]
internal class StopVerb : IAction
{
    public void Action()
    {
        Console.WriteLine("Shutdown lisener tasks...");
        Program.host?.Stop();
    }
}
