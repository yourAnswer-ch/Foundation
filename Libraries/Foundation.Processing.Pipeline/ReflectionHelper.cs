using System.ComponentModel;
using System.Reflection;

namespace Foundation.Processing.Pipeline;

internal static class ReflectionHelper
{
    public static IEnumerable<KeyValuePair<string, object?>> GetProperties(this object? values)
    {
        if (values == null)
            yield break;

        var properies = TypeDescriptor.GetProperties(values);
        foreach (PropertyDescriptor property in properies)
        {
            yield return new KeyValuePair<string, object?>(property.Name, property.GetValue(values));
        }
    }

    public static MethodInfo? FindMethod<T>(this T command, string name) where T : ICommand
    {
        if (command == null)
            throw new ArgumentException("FindMethod - parameter command can not be null");

        var type = command.GetType();
        return type.GetRuntimeMethods().SingleOrDefault(e => Match(e, type));

        bool Match(MethodInfo info, Type commandType)
        {
            if (!string.Equals(info.Name, name, StringComparison.InvariantCultureIgnoreCase))
                return false;

            if (!info.IsPublic || info.DeclaringType != commandType)
                return false;

            return true;
        }
    }

    public static IEnumerable<object> GetParameters(this IPipelineContext context, MethodInfo methodInfo)
    {
        var parameters = methodInfo.GetParameters();

        foreach (var parameter in parameters)
        {
            if (parameter.Name == null)
                continue;

            var value = context.GetParameter(parameter.Name, parameter.ParameterType);

            if (value == null)
                throw new ArgumentException($"Parameter: {parameter.Name} not found or null.");
               
            yield return value;
        }
    }

    public static object? GetResult(this IAsyncResult result)
    {
        var property = result.GetType().GetRuntimeProperty("Result");
        return property?.GetValue(result);
    }
}