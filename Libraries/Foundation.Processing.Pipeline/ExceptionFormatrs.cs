using System.Reflection;

namespace Foundation.Processing.Pipeline;

internal class ExceptionFormatrs : Dictionary<Type, Func<Exception, string, string>>
{
    public string Format(Exception ex, string commandName)
    {
        var exception = (ex is TargetInvocationException) ? ex.InnerException : ex;
        if(exception == null)
            return $"Pipeline - command: '{commandName}' execution failed - unknown exception.";

        var type = exception.GetType();
        
        if (TryGetValue(type, out var function))
            return function.Invoke(exception, commandName);

        return $"Pipeline - command: '{commandName}' execution failed: {ex.Message}";
    }
}