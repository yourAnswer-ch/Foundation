using Foundation.Configuration.KeyVault;
using Foundation.Logging.EventHubLogger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Foundation.ServiceBuilder.AzureDefault
{
    public class DefaultAzureStack : Stack
    {
        public static new IStack Create => new DefaultAzureStack();

        public override IStack AddConfiguration(Action<IConfigurationBuilder>? builder = null)
        {
            var environment = Environment.GetEnvironmentVariable("ENVIRONMENT");

            if (string.IsNullOrWhiteSpace(environment))
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            return base.AddConfiguration(b =>
            {
                b.AddJsonFile("appsettings.json", false);
                if (!string.IsNullOrWhiteSpace(environment))
                    b.AddJsonFile($"appsettings.{environment}.json", true);                
                b.AddAzureKeyVault();
                builder?.Invoke(b);
            });
        }

        public override IStack AddLogging(Action<ILoggingBuilder, IConfiguration>? builder = null)
        {
            return base.AddLogging((b, c) =>
            {
                b.AddConsole();                
                b.AddEventHubLogger();
                b.AddTraceSource("Sherlock");
                b.AddConfiguration(c.GetSection("Logging"));
                builder?.Invoke(b, c);
            });
        }

        public override IStack AddLogging(Action<ILoggingBuilder>? builder = null)
        {
            return base.AddLogging((b, c) =>
            {
                b.AddConsole();
                b.AddEventHubLogger();
                b.AddTraceSource("Sherlock");
                b.AddConfiguration(c.GetSection("Logging"));
                builder?.Invoke(b);
            });
        }
    }
}