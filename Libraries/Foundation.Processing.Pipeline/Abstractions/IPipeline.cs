namespace Foundation.Processing.Pipeline.Abstractions;

public interface IPipeline
{
    Task ExecuteAsync(object parameters);
}