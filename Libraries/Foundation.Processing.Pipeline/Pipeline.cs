using Foundation.Processing.Pipeline.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using System.Diagnostics;

namespace Foundation.Processing.Pipeline;

public class Pipeline : IPipeline
{
    private readonly ILogger _log;
    private readonly IServiceProvider _provieder;
    private readonly IList<ICommandDefinition> _commands;

    internal Pipeline(IServiceProvider provider, IList<ICommandDefinition> commands)
    {
        _provieder = provider;
        _commands = commands;
        _log = provider.GetRequiredService<ILogger<Pipeline>>();        
    }

    public async Task ExecuteAsync(object? parameters = null)
    {
        var commandName = "";

        var policy = Policy.Handle<Exception>().RetryAsync(3, (e, r) =>
        {
            _log.LogWarning(e, $"Pipeline - Command: {commandName} failed. Retry attemp: {r}");
        });

        var properties = parameters.GetProperties();
        var context = new PipelineContext(properties);
        var queue = new Queue<ICommand>(_commands.Count);
        
        try
        {
            foreach (var definition in _commands)
            {
                var stopwatch = Stopwatch.StartNew();

                var command = definition.CreateCommand(_provieder);
                if (command == null)
                    throw new ArgumentException($"Pipeline - command: {definition.Name} could not create instance.");

                var rollback = definition.CreateRollbackCommand(_provieder);
                if(rollback != null)
                    queue.Enqueue(command);

                commandName = definition.Name;
                var result = await policy.ExecuteAsync(() => command.ExecuteAsync(context));

                if (result.FlowControl == FlowControl.Exit)
                {
                    _log.LogWarning("Pipeline - Exit pipeline.");
                    return;
                }

                stopwatch.Stop();
                _log.LogInformation($"Pipeline - Command: {definition.Name} successfuly executed. Duration {stopwatch.ElapsedMilliseconds}");
            }
        }
        catch(Exception ex)
        {           
            _log.LogError($"Pipeline - Command: {commandName} failed.", ex);
            await Rollback(queue, context);
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
                await command.ExecuteAsync(context);
            }
            catch (Exception ex)
            {
                _log.LogError($"Pipeline - Rollback command: {command.GetType().Name} fail too rollback.", ex);
            }
        }
    }
}