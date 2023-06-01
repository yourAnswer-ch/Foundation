using CommandLine;

namespace CloudLogger.Commands;

[Verb("filter", HelpText = "filter <command>, type 'filter help' for available commands")]
class FilterVerb : IAction
{
    [Value(0)]
    public required IEnumerable<string> SubArgs { get; set; }

    public void Action()
    {
        FilterSubVerbs.Parse(SubArgs);
    }
}
