namespace Foundation.Processing.Pipeline;

internal interface ICommandDefinition
{
    string Name { get; }

    ICommand CreateCommand(IServiceProvider provider);

    ICommand? CreateRollbackCommand(IServiceProvider provider);
}
