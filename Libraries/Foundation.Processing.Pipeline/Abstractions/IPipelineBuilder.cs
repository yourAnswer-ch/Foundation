namespace Foundation.Processing.Pipeline.Abstractions
{
    public interface IPipelineBuilder
    {
        void AddCommand<TCommand>() 
            where TCommand : ICommand;

        void AddCommand<TCommand, TRollback>() 
            where TCommand : ICommand 
            where TRollback : ICommand;

        void AddExceptionFormater<T>(Func<T, string, string> formatter)
            where T : Exception;
    }
}