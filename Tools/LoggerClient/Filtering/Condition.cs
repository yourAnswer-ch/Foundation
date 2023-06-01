using Foundation.Logging.EventHubLogger.Interface;

namespace CloudLogger.Filtering;

internal class Condition<T> : ICondition
{
    public string FilterTypeName { get; set; }

    public string FilterValue { get; set; }

    public Func<LogEntry, T?> GetField { get; set; }

    public Func<T,T,bool> Compare { get; set; }

    public List<T> Conditions { get; }

    public bool Match(LogEntry logEntry)
    {
        var value = GetField(logEntry);
        return value != null && Conditions.Any(e => Compare.Invoke(value, e));
    }

    public Condition(string FilterTypeName, string FilterValue, Func<LogEntry, T?> field, Func<T, T, bool> compare, params T[] conditions)
    {
        this.FilterTypeName = FilterTypeName;
        this.FilterValue = FilterValue;
        GetField = field;
        Compare = compare;
        Conditions = new List<T>(conditions);
    }

    public override string ToString()
    {
        return $"{FilterTypeName}={FilterValue}";
    }
}