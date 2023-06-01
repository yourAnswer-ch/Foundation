using CommandLine;
using System.Reflection;

namespace CloudLogger.Commands;

internal static class CommandVerbs
{
    private static readonly Type[] _mainVerbs = new List<Type>()
    {
        typeof(SpoolVerb),
        typeof(StopVerb),
        typeof(ExitVerb),
        typeof(ClearScreenVerb),
        typeof(FilterVerb),
        typeof(EnvironmentVerb),
        typeof(ReplayVerb),
        typeof(ShownameVerb)
    }.ToArray();


    private static void NotParsed(IEnumerable<Error> errors)
    {
        foreach (var error in errors)
        {
            switch(error)
            {
                case BadVerbSelectedError e:
                  Console.WriteLine($"Unknown Command '{e.Token}', type 'help' for available Commands.");
                  break;

                case HelpRequestedError _:
                    var h = new CommandLine.Text.HelpText();
                    h.AddVerbs(_mainVerbs);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(h.ToString().Trim(System.Environment.NewLine.ToCharArray()));
                    break;

                case HelpVerbRequestedError _:                        
                    var h2 = new CommandLine.Text.HelpText();
                    h2.AddVerbs(_mainVerbs);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(h2.ToString().Trim(System.Environment.NewLine.ToCharArray()));                       
                    break;

                case VersionRequestedError _:
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "UNKNOWN");
                    break;

                case BadFormatConversionError _:
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"Command parameter error");
                    break;

                case MissingRequiredOptionError _:
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"missing commend parameter");
                    break;

                default:
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"unexpected command error: {error}");
                    break;

            }
        }
    }

    public static void ParseStart(string[] value)
    {
        var parser = new Parser(settings =>
        {
            settings.EnableDashDash = false;
            settings.HelpWriter = null;
        });

        var result = parser.ParseArguments<EnvironmentVerb>(value)
            .WithNotParsed(NotParsed)
            .WithParsed(o => ((IAction)o).Action());              
    }

    public static void Parse(string value)
    {

        var parser = new Parser(settings =>
        {
            settings.EnableDashDash = false;
            settings.HelpWriter = null;
        });

        var result = parser.ParseArguments(value.Split(' '), _mainVerbs)
            .WithNotParsed(NotParsed)
            .WithParsed<IAction>(o => o.Action());
    }
}
