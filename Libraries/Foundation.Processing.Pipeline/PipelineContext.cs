namespace Foundation.Processing.Pipeline;

public class PipelineContext : IPipelineContext
{
    private readonly IDictionary<string, object?> _values;

    public PipelineContext(IDictionary<string, object?> parameters)
    {
        _values = new Dictionary<string, object?>(parameters, StringComparer.InvariantCultureIgnoreCase);
    }

    public PipelineContext(IEnumerable<KeyValuePair<string, object?>> parameters)
    {
        _values = new Dictionary<string, object?>(parameters, StringComparer.InvariantCultureIgnoreCase);
    }

    public PipelineContext()
    {
        _values = new Dictionary<string, object?>(StringComparer.InvariantCultureIgnoreCase);
    }

    public void AppParameter<T>(string name, T value)
    {
        _values.Add(name, value);
    }

    public T GetParameter<T>(string name)
    {
        if(_values[name] is T value)
            return value;

        throw new ArgumentException($"Parameter: {name} is null.");
    }

    public object? GetParameter(string name, Type type)
    {
        if(_values.ContainsKey(name))
            return _values[name];

        return null;
    }
}
