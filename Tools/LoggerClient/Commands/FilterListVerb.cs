using CommandLine;

namespace CloudLogger.Commands;

[Verb("list", HelpText = "list filters")]
internal class FilterListVerb : IAction
{
    public void Action()
    {
        Program.filter?.Conditions.ForEach(Console.WriteLine);
    }
}
