using CommandLine;

namespace CloudLogger.Commands;

[Verb("add", HelpText = "set a filter --> add <type> <value> --> types:(LogLevel/MinLogLevel/App/Host/Type/!Type/CorrelationId)")]
internal class FilterAddVerb : IAction
{
    [Value(0, Required = true)]
    public string? Type { get; set; }

    [Value(1, Required = true)]
    public string? Value { get; set; }

    public void Action()
    {
        var filterString = $"{Type}={Value}";
        try
        {
            Program.filter?.Parse(filterString);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Cannot apply filter: '{filterString}' error: {ex.Message}");
            Console.ForegroundColor = ConsoleColor.White;
        }
        finally
        {
            Program.SetHeadline();
            Console.WriteLine($"Filter: {Program.filter}");
            Task.Delay(TimeSpan.FromSeconds(5)).Wait();
            Console.WriteLine("Resume listening to log stream...");
        }
    }
}
