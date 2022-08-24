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
    private readonly ExceptionFormatrs _formaters;

    public IList<Exception> Exceptions { get; private set; }

    internal Pipeline(IServiceProvider provider, IList<ICommandDefinition> commands, ExceptionFormatrs formaters)
    {
        _provieder = provider;
        _commands = commands;
        _log = provider.GetRequiredService<ILogger<Pipeline>>();
        _formaters = formaters;
        Exceptions = new List<Exception>();
    }

    public async Task<bool> ExecuteAsync(object? parameters = null)
    {
        var commandName = "";

        var policy = Policy.Handle<Exception>().RetryAsync(3, (ex, r) =>
        {
            var message = _formaters.Format(ex, commandName);
            _log.LogWarning($"Pipeline - Command: {commandName} failed. Retry attemp: {r}");
            _log.LogWarning(ex, message);
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
                var result = await policy.ExecuteAsync(async () =>
                {
                    var result = await command.ExecuteAsync(context);

                    if (result.FlowControl == FlowControl.Failed)
                        throw new InvalidOperationException($"Command: {commandName} returned failed state.");

                    return result;
                });

                if (result.FlowControl == FlowControl.Exit)
                {
                    _log.LogWarning("Pipeline - Exit pipeline.");
                    return false;
                }                 

                stopwatch.Stop();
                _log.LogInformation($"Pipeline - Command: {definition.Name} successfuly executed. Duration {stopwatch.ElapsedMilliseconds}");
            }

            return true;
        }
        catch(Exception ex)
        {
            Exceptions.Add(ex);
            
            _log.LogError(_formaters.Format(ex, commandName), ex);
            
            await Rollback(queue, context);

            return false;
        }
    }

    private async Task Rollback(Queue<ICommand> queue, IPipelineContext context)
    {
        try
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
        catch(Exception ex)
        {
            _log.LogWarning(ex, $"Pipeline - Rollback failed: {ex.Message}");
        }
    }
}