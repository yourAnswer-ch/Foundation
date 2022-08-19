using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Foundation.ServiceBuilder
{
    public interface IStack
    {
        IStack AddConfiguration();

        IStack AddConfiguration(Action<IConfigurationBuilder>? builder);

        IStack AddLogging();

        IStack AddLogging(Action<ILoggingBuilder>? builder);

        IStack AddLogging(Action<ILoggingBuilder, IConfiguration>? builder);

        IStack AddServices(Action<IServiceCollection>? builder);

        IServiceProvider Build(Action<IServiceProvider>? initializer = null);
    }
}