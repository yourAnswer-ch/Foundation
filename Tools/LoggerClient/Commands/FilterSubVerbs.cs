using CommandLine;

namespace CloudLogger.Commands;

internal static class FilterSubVerbs
{
    private static readonly Type[] FilterVerbs = {
        typeof(FilterListVerb),
        typeof(FilterClearVerb),
        typeof(FilterAddVerb),
    };

    private static void NotParsed(IEnumerable<Error> errors)
    {
        foreach (var e in errors)
        {
            if (e is BadVerbSelectedError || e is VersionRequestedError)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Unknown Command '{((BadVerbSelectedError)e).Token}'");
            }
            else if (e is HelpVerbRequestedError || e is HelpRequestedError)
            {
                var h = new CommandLine.Text.HelpText() { AutoHelp = false, AutoVersion = false };
                h.AddVerbs(FilterVerbs);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(h.ToString().Trim(System.Environment.NewLine.ToCharArray()));
            }
        }
    }

    public static void Parse(IEnumerable<string> subArgs)
    {
        var filterResult = new Parser(settings =>
        {
            settings.EnableDashDash = false;
            settings.HelpWriter = null;

        })
        .ParseArguments<FilterListVerb, FilterClearVerb, FilterAddVerb>(subArgs);


        filterResult.WithParsed(o =>
        {
            ((IAction)o).Action();
        });


        filterResult.WithNotParsed(NotParsed);
    }
}
