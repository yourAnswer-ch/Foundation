using CommandLine;

namespace CloudLogger.Commands;

[Verb("cls", HelpText = "clear screen")]
internal class ClearScreenVerb : IAction
{
    public void Action()
    {
        Console.Clear();
    }
}
