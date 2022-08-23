using Microsoft.Extensions.DependencyInjection;

namespace Foundation.Processing.Pipeline;

internal class CommandDefinition<TCommand> : ICommandDefinition where TCommand : ICommand
{
    public string Name => typeof(TCommand).Name;

    public ICommand CreateCommand(IServiceProvider provider)
    {
        return ActivatorUtilities.CreateInstance<TCommand>(provider);
    }

    public ICommand? CreateRollbackCommand(IServiceProvider provider)
    {
        return null;
    }
}

internal class CommandDefinition<TCommand, TRollback> : ICommandDefinition where TCommand : ICommand where TRollback : ICommand
{
    public string Name => typeof(TCommand).Name;

    public ICommand CreateCommand(IServiceProvider provider)
    {
        return ActivatorUtilities.CreateInstance<TCommand>(provider);
    }

    public ICommand? CreateRollbackCommand(IServiceProvider provider)
    {
        return ActivatorUtilities.CreateInstance<TRollback>(provider);
    }
}
