using System.Reflection;

namespace Foundation.Processing.Pipeline;

public abstract class Command : ICommand
{
    private MethodInfo? executeAsync;
    private MethodInfo? rollbackAsync;

    public Command()
    {
        executeAsync = this.FindMethod("ExecuteAsync");
        rollbackAsync = this.FindMethod("RollbackAsync");
    }

    public async Task ExecuteAsync(IPipelineContext context)
    {
        await Execute(executeAsync, context);
    }

    public async Task RollbackAsync(IPipelineContext context)
    {
        await Execute(rollbackAsync, context);
    }

    private async Task Execute(MethodInfo? method, IPipelineContext context)
    {
        if (method == null)
            throw new ArgumentException("Command - ExecuteAsync method not found.");

        var parameters = context.GetParameters(method).ToArray();

        await Task.Factory.FromAsync((IAsyncResult)method.Invoke(this, parameters), e =>
        {
            context.Update(e.GetResult().GetProperties());
        });
    }
}
