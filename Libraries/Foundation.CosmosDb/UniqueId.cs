namespace Foundation.CosmosDb;

public static class UniqueId
{
    public static string Create() => Guid.CreateVersion7().ToBase62();
}