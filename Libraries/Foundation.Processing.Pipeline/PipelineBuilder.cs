using Foundation.Processing.Pipeline.Abstractions;

namespace Foundation.Processing.Pipeline;

public class PipelineBuilder : IPipelineBuilder
{
    internal IList<ICommandDefinition> Definitions { get; }

    public PipelineBuilder()
    {
        Definitions = new List<ICommandDefinition>();
    }

    public void AddCommand<T>() where T : ICommand
    {
        Definitions.Add(new CommandDefinition<T>());
    }

    public void AddCommand<TCommand, TRollback>() where TCommand : ICommand  where TRollback : ICommand
    {
        Definitions.Add(new CommandDefinition<TCommand, TRollback>());
    }
}