namespace Foundation.Processing.Pipeline.Abstractions
{
    internal interface IPipeDefinition
    {
        string Name { get; }

        ICommand CreateCommand(IServiceProvider provider);
    }
}