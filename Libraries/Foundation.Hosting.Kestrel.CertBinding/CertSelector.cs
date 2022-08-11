using Foundation.Hosting.Kestrel.CertBinding.Configuration;
using Microsoft.AspNetCore.Connections;
using System.Security.Cryptography.X509Certificates;

namespace Foundation.Hosting.Kestrel.CertBinding;

internal class CertSelector
{
    private Dictionary<string, X509Certificate2> certs = new Dictionary<string, X509Certificate2>(StringComparer.OrdinalIgnoreCase);

    public CertSelector(IEnumerable<CertificateConfig> configurations, Func<CertificateConfig, X509Certificate2> downloader)
    {
        certs = new Dictionary<string, X509Certificate2>(StringComparer.OrdinalIgnoreCase);

        foreach (var config in configurations)
        {
            if (string.IsNullOrEmpty(config.HostHeader))
                throw new ArgumentException("CertSelector - HostHeader must be specified to work with SIN.");

            var cert = downloader.Invoke(config);
            certs.Add(config.HostHeader, cert);
        }

    }

    public X509Certificate2 SelectCert(ConnectionContext connectionContext, string name)
    {
        if (name is not null && certs.TryGetValue(name, out var cert))
        {
            return cert;
        }

        throw new ArgumentException($"CertSelector - certificate not found for name: {name}");
    }
}