using Foundation.CosmosDb.Options;
using Microsoft.Extensions.DependencyInjection;


namespace Foundation.CosmosDb;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCosmosDb(
        this IServiceCollection services,
        Action<CosmosDbOptions> configureOptions)
    {
        services.AddOptions<CosmosDbOptions>()
            .Configure(configureOptions)
            .ValidateDataAnnotations();

        services.AddSingleton<ICosmosDb, CosmosDb>(CosmosDb.Create);

        return services;
    }

    public static IServiceCollection AddCosmosDb<T>(
        this IServiceCollection services,
        Action<CosmosDbOptions> configureOptions) where T : CosmosDb, new()
    {
        services.AddOptions<CosmosDbOptions>(typeof(T).Name)
            .Configure(configureOptions)
            .ValidateDataAnnotations();

        services.AddSingleton<ICosmosDb, T>(CosmosDb.Create<T>);

        return services;
    }

    public static IServiceCollection AddCosmosDbContainer<T>(
        this IServiceCollection services) where T : class, ICosmosDbContainer
    {

        services.AddSingleton<T>();

        return services;
    }
}
