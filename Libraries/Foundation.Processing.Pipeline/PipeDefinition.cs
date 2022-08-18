using Foundation.Processing.Pipeline.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Foundation.Processing.Pipeline;

internal class PipeDefinition<T> : IPipeDefinition where T : ICommand
{
    public string Name => typeof(T).Name;

    public ICommand CreateCommand(IServiceProvider provider)
    {
        return ActivatorUtilities.CreateInstance<T>(provider);
    }

    public PipeDefinition()
    {
    }
}