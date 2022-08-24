namespace Foundation.Processing.Pipeline.Abstractions;

public interface IPipeline
{
    IList<Exception> Exceptions { get; }

    Task<bool> ExecuteAsync(object parameters);
}