using Azure.Security.KeyVault.Certificates;
using Foundation.Hosting.Kestrel.CertBinding.Configuration;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography.X509Certificates;

namespace Foundation.Hosting.Kestrel.CertBinding;

public class CertificationStore
{
    private ILogger _log;
    private KestrelConfig _config;    
    private IAzureClientFactory<CertificateClient> _factory;
    
    private List<CertificateEntry> _certificates = new();
    
    public CertificationStore(
        IAzureClientFactory<CertificateClient> factory, 
        IConfiguration config, 
        ILogger<CertificationStore> log)
    {
        _log = log;
        _factory = factory;
        _config = config.GetKestrelConfig();
    }

    internal X509Certificate2 GetCertificat(string name)
    {
        if (_certificates.Count == 0)
            throw new ArgumentException("No Certificates available");

        if (string.IsNullOrWhiteSpace(name))
            return _certificates.First().Certificate;

        var cert = _certificates.FirstOrDefault(x => x.Matches(name));
        if (cert != null)
            return cert.Certificate;

        cert = _certificates.First();
        _log.LogWarning($"No matching certificate found for host: '{name}'. Certificate: {cert} got selected.");
        
        return cert.Certificate;
    }

    internal void LoadCertificates()
    {
        try
        {
            var newCertificates = new List<CertificateEntry>();

            foreach (var bindings in _config.Bindings)
            {
                if(bindings.Certificate == null)
                    continue;

                if (bindings.Certificate.Source == null)
                    throw new ArgumentException("Source not defind.");

                foreach (var name in bindings.Certificate.Names)
                {                    
                    var entry = DownloadCertificate(bindings.Certificate.Source, name);
                    newCertificates.Add(entry);
                }
            }

            _certificates = newCertificates;
        }
        catch (Exception ex) {
            _log.LogError(ex, "Fail to retrieve certificates");
        }
    }

    private CertificateEntry DownloadCertificate(string source, string name)
    {
        var client = _factory.CreateClient(source);
        if (client == null)
            throw new ArgumentException($"ServerCertBinder - CertificateClient not found - Name: {name}");

        X509Certificate2 cert = client.DownloadCertificate(name);
        var verify = cert.Verify();
        
        var dnsName = cert.GetNameInfo(X509NameType.DnsName, false);
        var alternativeNames = cert.GetSubjectAlternativeNames();

        var hosts = alternativeNames.Concat(new[] { dnsName }).Distinct();
        var entry = new CertificateEntry(cert, hosts);

        _log.Log(
            verify ? LogLevel.Information : LogLevel.Warning,
            $"Retrieved certificate - Name: {name} - Source: {source} - DNS Name: {entry} - Verify: {verify}");

        return entry;
    }
}
