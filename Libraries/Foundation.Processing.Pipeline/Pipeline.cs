using Foundation.Processing.Pipeline.Abstractions;
using Microsoft.Extensions.Logging;
using Polly;
using System.Diagnostics;

namespace Foundation.Processing.Pipeline;

public class Pipeline : IPipeline
{
    private readonly ILogger _log;
    private readonly IList<IPipeDefinition> _pipeDefinitions;

    internal Pipeline(IList<IPipeDefinition> pipeDefinitions, ILogger<Pipeline> log)
    {
        _log = log;
        _pipeDefinitions = pipeDefinitions;
    }

    public async Task ExecuteAsync(IServiceProvider provieder, object? parameters)
    {
        var commandName = "";

        var policy = Policy.Handle<Exception>().RetryAsync(3, (e, r) =>
        {
            _log.LogWarning(e, $"Pipeline - Command: {commandName} failed. Retry attemp: {r}");
        });

        var properties = parameters.GetProperties();
        var context = new PipelineContext(properties);
        var queue = new Queue<ICommand>(_pipeDefinitions.Count);
        
        try
        {
            foreach (var definition in _pipeDefinitions)
            {
                var stopwatch = Stopwatch.StartNew();

                var command = definition.CreateCommand(provieder);
                if (command == null)
                    throw new ArgumentException($"Pipeline - command: {definition.Name} could not create instance.");
                
                commandName = definition.Name;
                await policy.ExecuteAsync(() => command.ExecuteAsync(context));

                stopwatch.Stop();
                _log.LogInformation($"Pipeline - Command: {definition.Name} successfuly executed. Duration {stopwatch.ElapsedMilliseconds}");
            }
        }
        catch(Exception ex)
        {
            _log.LogError($"Pipeline - Command: {commandName} failed.", ex);
        }
    }

    private async Task Rollback(Queue<ICommand> queue, IPipelineContext context)
    {
        _log.LogWarning("Pipeline - Start rollback");

        while (queue.Count > 0)
        {
            var command = queue.Dequeue();
            try
            {
                await command.RollbackAsync(context);
            }
            catch (Exception ex)
            {
                _log.LogError($"Pipeline - Rollback command: {command.GetType().Name} fail too rollback.", ex);
            }
        }
    }
}