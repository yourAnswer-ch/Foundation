using Foundation.Processing.Pipeline.Abstractions;

namespace Foundation.Processing.Pipeline;

public class PipelineBuilder : IPipelineBuilder
{
    internal IList<ICommandDefinition> Definitions { get; }

    internal ExceptionFormatrs Formaters { get; }

    public PipelineBuilder()
    {
        Definitions = new List<ICommandDefinition>();
        Formaters = new ExceptionFormatrs();
    }

    public void AddCommand<T>() where T : ICommand
    {
        Definitions.Add(new CommandDefinition<T>());
    }

    public void AddCommand<TCommand, TRollback>() where TCommand : ICommand  where TRollback : ICommand
    {
        Definitions.Add(new CommandDefinition<TCommand, TRollback>());
    }

    public void AddExceptionFormater<T>(Func<T, string, string> formatter) where T : Exception
    {
        Formaters.Add(typeof(T), (e, c) => formatter((T)e, c) );
    }
}
