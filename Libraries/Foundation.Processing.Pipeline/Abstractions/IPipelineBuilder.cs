namespace Foundation.Processing.Pipeline.Abstractions
{
    public interface IPipelineBuilder
    {
        void AddCommand<T>() where T : ICommand;
    }
}