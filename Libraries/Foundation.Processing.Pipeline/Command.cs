using System.Reflection;

namespace Foundation.Processing.Pipeline;

public abstract class Command : ICommand
{
    private MethodInfo? executeAsync;   

    public Command()
    {
        executeAsync = this.FindMethod("ExecuteAsync");
    }

    public async Task<Result> ExecuteAsync(IPipelineContext context)
    {
        return await Execute(executeAsync, context);
    }

    private async Task<Result> Execute(MethodInfo? method, IPipelineContext context)
    {
        if (method == null)
            throw new ArgumentException("Command - ExecuteAsync method not found.");

        var parameters = context.GetParameters(method).ToArray();

        var result = await (Task<Result>)method.Invoke(this, parameters);

        if(result.Value != null)
            context.Update(result.Value.GetProperties());

        return result;
    }
}
