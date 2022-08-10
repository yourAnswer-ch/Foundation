using Azure.Security.KeyVault.Certificates;
using Foundation.Hosting.Kestrel.CertBinding.Configuration;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Foundation.Hosting.Kestrel.CertBinding;

public static class ServerCertBinder
{
    public static void ConfigureBindings(this KestrelServerOptions options)
    {
        var rootconfig = options.ApplicationServices.GetRequiredService<IConfiguration>();
        var config  = rootconfig.GeKestrelConfig();
        var client = options.ApplicationServices.GetRequiredService<IAzureClientFactory<CertificateClient>>().CreateClient("KV-FD-Certificates");
        X509Certificate2 certificate = client.DownloadCertificate("wildcard-youranswer-ch");
       
        options.AddServerHeader = false;
        options.Listen(IPAddress.Loopback, 10443, o =>
        {
            o.Protocols = HttpProtocols.Http1AndHttp2;

            o.UseHttps(httpsConfig =>
            {
                httpsConfig.SslProtocols = SslProtocols.Tls13 | SslProtocols.Tls12;
                httpsConfig.ServerCertificate = certificate;

                //httpsConfig.ServerCertificateSelector +=
            });
        });
    }
}

public class CertSelector
{
    private Dictionary<string, X509Certificate2> certs = new Dictionary<string, X509Certificate2>(StringComparer.OrdinalIgnoreCase);
    //{
    //    ["localhost"] = localhostCert,
    //    ["example.com"] = exampleCert,
    //    ["sub.example.com"] = subExampleCert
    //};

    public X509Certificate2 SelectCert(ConnectionContext connectionContext, string name)
    {
        if (name is not null && certs.TryGetValue(name, out var cert))
        {
            return cert;
        }

        return null;
    }
}