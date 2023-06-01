using CommandLine;

namespace CloudLogger.Commands;

[Verb("clear", HelpText = "clear filters")]
internal class FilterClearVerb : IAction
{
    public void Action()
    {
        Program.filter?.Clear();
        Program.SetHeadline();
        Task.Delay(TimeSpan.FromSeconds(5)).Wait();
        Console.WriteLine("Resume listening to log stream...");
    }
}
