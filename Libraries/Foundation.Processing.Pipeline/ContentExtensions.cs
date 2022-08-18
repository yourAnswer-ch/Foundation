namespace Foundation.Processing.Pipeline;

internal static class ContentExtensions
{
    public static void Update(this IPipelineContext context, IEnumerable<KeyValuePair<string, object?>>? values)
    {
        if (values == null)
            return;

        foreach (var item in values)
        {
            context.AppParameter(item.Key, item.Value);
        }
    }
}