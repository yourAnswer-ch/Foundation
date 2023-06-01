using CommandLine;

namespace CloudLogger.Commands;

[Verb("exit", HelpText = "exit app")]
internal class ExitVerb : IAction
{
    public void Action()
    {
        Program.leave = true;
    }
}
