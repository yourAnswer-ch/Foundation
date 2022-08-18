public interface IPipelineContext
{
    void AppParameter<T>(string name, T value);

    T GetParameter<T>(string name);

    object? GetParameter(string name, Type type);
}
