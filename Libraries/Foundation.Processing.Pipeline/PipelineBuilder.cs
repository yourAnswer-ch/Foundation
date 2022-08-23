using Foundation.Processing.Pipeline.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Foundation.Processing.Pipeline;

public class PipelineBuilder : IPipelineBuilder
{
    IServiceCollection Services { get; }

    internal IList<ICommandDefinition> Definitions => _pipeDefinitions;

    private readonly IList<ICommandDefinition> _pipeDefinitions;

    public PipelineBuilder(IServiceCollection services)
    {
        Services = services;
        _pipeDefinitions = new List<ICommandDefinition>();

    }

    public void AddCommand<T>() where T : ICommand
    {
        _pipeDefinitions.Add(new CommandDefinition<T>());
    }

    public void AddCommand<TCommand, TRollback>() where TCommand : ICommand  where TRollback : ICommand
    {
        _pipeDefinitions.Add(new CommandDefinition<TCommand, TRollback>());
    }
}