using Azure.Security.KeyVault.Certificates;
using Foundation.Hosting.Kestrel.CertBinding.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

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
        catch (Exception ex) {
            log.LogError(ex, "Fail to configure kestrel server.");
            throw;
        }
    }

    private static void AddListener(KestrelServerOptions options, IServiceProvider services, KestrelBindingConfig binding)
    {
        var ip = ParseAddressIP(binding);
        var certificate = (binding.Certificate != null) ? DownloadCertificate(services, binding.Certificate) : null;
        
        options.Listen(ip, binding.Port, o =>
        {
            var httpProtocol = ParseEnum(binding.Protocols, HttpProtocols.Http1AndHttp2);
            var sslProtocol = ParseEnum(binding.Certificate?.Protocols, SslProtocols.Tls13);

            o.Protocols = httpProtocol;
            if (certificate != null)
            {
                o.UseHttps(httpsConfig =>
                {
                    httpsConfig.SslProtocols = sslProtocol;

                    if (certificate != null)
                        httpsConfig.ServerCertificate = certificate;

                    if (binding.Certificates != null && binding.Certificates.Count > 0)
                    {
                        var selector = new CertSelector(binding.Certificates, c => DownloadCertificate(services, c));
                        httpsConfig.ServerCertificateSelector = selector.SelectCert;
                    }
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

    private static X509Certificate2 DownloadCertificate(IServiceProvider services, CertificateConfig config)
    {
        var factory = services.GetRequiredService<IAzureClientFactory<CertificateClient>>();
        if (factory == null)
            throw new ArgumentException("ServerCertBinder - AzureClientFactory for CertificateClient found");

        var client = factory.CreateClient(config.Source);
        if (factory == null)
            throw new ArgumentException($"ServerCertBinder - CertificateClient not found - Name: {config.Name}");

        return client.DownloadCertificate(config.Name);
    }

    private static T ParseEnum<T>(string? value, T defaultValue = default) where T : struct
    {
        if(string.IsNullOrEmpty(value))
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
