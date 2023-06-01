using Foundation.Logging.EventHubLogger.Interface;
using Microsoft.Extensions.Logging;

namespace CloudLogger.Filtering;

internal class LogFilter
{
    internal List<ICondition> Conditions { get; }

    public LogFilter()
    {
        Conditions = new List<ICondition>();
    }

    public bool Match(LogEntry entry)
    {
        if (entry == null)
            return false;

        if (!Conditions.Any())
            return true;

        return Conditions.All(e => e.Match(entry));
    }

    public void Clear()
    {
        Conditions.Clear();
    }

    public void Parse(string filerstring)
    {

        if (string.IsNullOrWhiteSpace(filerstring))
        {
            Clear();
            return;
        }

        var contitions = new List<ICondition>();

        var segments = filerstring.Split(';');

        foreach (var segment in segments)
        {
            var expr = segment.Split('=');
            if (expr.Length != 2)
                throw new ArgumentException($"Invalid Filter expression: {segment}");

            var command = expr[0];
            var values = expr[1];

            contitions.Add(ToCondition(command, values));
        }

        //Conditions.Clear();
        Conditions.AddRange(contitions);
    }

    private static ICondition ToCondition(string command, string value)
    {
        command = command.Trim().ToLower();
        switch (command)
        {
            case "loglevel":
                return new Condition<LogLevel>("loglevel", value, e => e.LogLevel, (a, b) => a == b, ToLogValues(value));

            case "minloglevel":
                return new Condition<LogLevel>("minloglevel", value, e => e.LogLevel, (a, b) => a >= b, ToLogValues(value));

            case "eventid":
                return new Condition<int>("eventid", value, e => e.EventId, (a, b) => a == b, ToIntValues(value));

            case "eventname":
                return new Condition<string>("eventname", value, e => e.EventName, (a, b) => string.Equals(a, b, StringComparison.OrdinalIgnoreCase), ToStringValues(value));

            case "host":
                return new Condition<string>("host", value, e => e.Host, (a, b) => string.Equals(a, b, StringComparison.OrdinalIgnoreCase), ToStringValues(value));

            case "app":
                return new Condition<string>("app", value, e => e.App, (a, b) => string.Equals(a, b, StringComparison.OrdinalIgnoreCase), ToStringValues(value));

            case "type":
                return new Condition<string>("type", value, e => e.Name, (a, b) => a.StartsWith(b, StringComparison.OrdinalIgnoreCase), ToStringValues(value));

            case "!type":
                return new Condition<string>("!type", value, e => e.Name, (a, b) => !a.StartsWith(b, StringComparison.OrdinalIgnoreCase), ToStringValues(value));

            case "correlationid":
                return new Condition<string>("correlationid", value, e => e.CorrelationId, (a, b) => string.Equals(a, b, StringComparison.OrdinalIgnoreCase), ToStringValues(value));

            default:
                throw new ArgumentException($"'{command}' is not a valid command.");
        }
    }

    private static string[] ToStringValues(string value)
    {
        try
        {
            var values = value.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            return values.Select(e => e.Trim()).ToArray();
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Fail to convert values: {value}.", ex);
        }
    }

    private static int[] ToIntValues(string value)
    {
        try
        {
            var values = value.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            return values.Select(e => Convert.ToInt32(e.Trim())).ToArray();
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Fail to convert int values: {value}.", ex);
        }
    }

    private static LogLevel[] ToLogValues(string value)
    {
        try
        {
            var values = value.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            return values.Select(e => (LogLevel)Enum.Parse(typeof(LogLevel), e.Trim(), true)).ToArray();
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Fail to convert LogLevel values: {value}.", ex);
        }
    }

    public override string ToString()
    {
        return string.Join(',', Conditions.Select(c => c.ToString()).ToArray());
    }
}
