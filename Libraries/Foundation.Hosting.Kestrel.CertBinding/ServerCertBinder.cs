using Foundation.Hosting.Kestrel.CertBinding.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Security.Authentication;

namespace Foundation.Hosting.Kestrel.CertBinding;

public static class ServerCertBinder
{
    public static void ConfigureBindings(this KestrelServerOptions options)
    {
        var services = options.ApplicationServices;
        var log = services.GetService<ILogger<KestrelConfig>>();
        var config = services.GetService<IConfiguration>();

        try
        {
            if (config == null)
                throw new ArgumentException("ServerCertBinder - Configuration not registered in service collection");

            var kestrelConfig = config.GetKestrelConfig();
            options.AddServerHeader = kestrelConfig.AddServerHeader;

            foreach (var binding in kestrelConfig.Bindings)
            {
                AddListener(options, services, binding);
            }
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Fail to configure kestrel server.");
            throw;
        }
    }

    private static void AddListener(KestrelServerOptions options, IServiceProvider services, KestrelBindingConfig binding)
    {
        var ip = ParseAddressIP(binding);

        options.Listen(ip, binding.Port, o =>
        {
            o.Protocols = ParseEnum(binding.Protocols, HttpProtocols.Http1AndHttp2);

            if (binding.Certificate != null)
            {
                o.UseHttps(httpsConfig =>
                {
                    httpsConfig.SslProtocols = ParseEnum(binding.Certificate.Protocols, SslProtocols.Tls13);

                    var sel = ActivatorUtilities.CreateInstance<CertSelector>(services);
                    httpsConfig.ServerCertificateSelector = sel.SelectCert;
                });
            }
        });
    }

    private static IPAddress ParseAddressIP(KestrelBindingConfig binding)
    {
        if (binding.IPAddress == "*")
            return IPAddress.Any;

        if (IPAddress.TryParse(binding.IPAddress, out var ipAddress))
            return ipAddress;

        return IPAddress.Any;
    }

    private static T ParseEnum<T>(string? value, T defaultValue = default) where T : struct
    {
        if (string.IsNullOrEmpty(value))
            return defaultValue;

        var result = value.Split('|').Select(e =>
        {
            if (!Enum.TryParse<T>(e, true, out var enumValue))
            {
                var names = string.Join('|', Enum.GetNames(typeof(T)));
                throw new ArgumentException($"ServerCertBinder - Value: {e} is not valid for type: '{typeof(T).Name}'. Use one of the following values: {names}.");
            }

            return Convert.ToInt32(enumValue);
        }).Aggregate((a, b) => a | b);

        return (T)Enum.ToObject(typeof(T), result);
    }
}
