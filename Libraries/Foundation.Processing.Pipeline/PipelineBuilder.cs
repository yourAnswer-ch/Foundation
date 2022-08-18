using Foundation.Processing.Pipeline.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Foundation.Processing.Pipeline;

public class PipelineBuilder : IPipelineBuilder
{
    IServiceCollection Services { get; }

    internal IList<IPipeDefinition> Definitions => _pipeDefinitions;

    private readonly IList<IPipeDefinition> _pipeDefinitions;

    public PipelineBuilder(IServiceCollection services)
    {
        Services = services;
        _pipeDefinitions = new List<IPipeDefinition>();

    }
    public void AddCommand<T>() where T : ICommand
    {
        _pipeDefinitions.Add(new PipeDefinition<T>());
    }
}