using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Foundation.ServiceBuilder
{
    public class Stack : IStack
    {
        private IConfigurationBuilder? _configurationBuilder;
        private IServiceCollection _servicesCollection;

        protected Stack()
        {
            _servicesCollection = new ServiceCollection();
        }

        public static IStack Create => new Stack();

        public virtual IStack AddConfiguration()
        {
            return AddConfiguration(null);
        }

        public virtual IStack AddConfiguration(Action<IConfigurationBuilder>? builder)
        {
            _configurationBuilder = new ConfigurationBuilder();
            builder?.Invoke(_configurationBuilder);
            return this;
        }

        public virtual IStack AddLogging()
        {
            return AddLogging(_ => { });
        }

        public virtual IStack AddLogging(Action<ILoggingBuilder>? builder)
        {
            if (_configurationBuilder == null)
                throw new ArgumentException("Stack - Add configuration bevore adding logging.");

            _servicesCollection.AddLogging(b =>
            {
                builder?.Invoke(b);
            });
            
            return this;
        }

        public virtual IStack AddLogging(Action<ILoggingBuilder, IConfiguration>? builder)
        {
            if (_configurationBuilder == null)
                throw new ArgumentException("Stack - Add configuration bevore adding logging.");

            var configuration = (IConfiguration)_configurationBuilder.Build();
            _servicesCollection.AddSingleton(configuration);
            _servicesCollection.AddLogging(b =>
            {
                builder?.Invoke(b, _configurationBuilder.Build());
            });

            return this;
        }

        public virtual IStack AddServices(Action<IServiceCollection>? builder = null)
        {
            builder?.Invoke(_servicesCollection);
            return this;
        }

        public virtual IServiceProvider Build(Action<IServiceProvider>? initializer = null)
        {
            var provider = _servicesCollection.BuildServiceProvider();
            initializer?.Invoke(provider);
            return provider;
        }
    }
}