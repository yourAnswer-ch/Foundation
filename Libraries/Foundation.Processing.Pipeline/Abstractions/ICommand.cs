namespace Foundation.Processing.Pipeline;

public interface ICommand
{
    Task<Result> ExecuteAsync(IPipelineContext context);
}
