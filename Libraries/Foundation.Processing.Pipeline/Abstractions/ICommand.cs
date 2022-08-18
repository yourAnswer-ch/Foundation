public interface ICommand
{
    Task ExecuteAsync(IPipelineContext context);

    Task RollbackAsync(IPipelineContext context);
}
