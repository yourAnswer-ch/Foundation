using Azure.Security.KeyVault.Certificates;
using Foundation.Azure.CertManager.Core.Configuration;
using Foundation.Processing.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Foundation.Azure.CertManager.Core.Steps;

public class AzureCheckIfIsExpired : Command
{
    private readonly ILogger _log;
    private readonly CertificateClient _client;
    private readonly CertificatesConfig _config;
    
    public AzureCheckIfIsExpired(CertificateClient client, IConfiguration config, ILogger<AzureCheckIfIsExpired> log)
    {
        _log = log;
        _client = client;
        _config = config.GetCertManagerConfig();
    }

    public Task<Result> ExecuteAsync(CertificateConfig domain)
    {
        if (string.IsNullOrWhiteSpace(domain?.DomainName))
            throw new ArgumentException("Domain name can not be null.");
       
        var versions = _client.GetPropertiesOfCertificateVersions(domain.CertificatName);
        var version = versions.OrderByDescending(e => e.CreatedOn).FirstOrDefault();

        if (version?.ExpiresOn == null)
        {
            _log.LogWarning($"{domain.DomainName} - Certificate version attritute not found. Start requesting certificate.");
            return Task.FromResult(Result.Next());
        }

        var expires = version.ExpiresOn;
        var until = DateTime.UtcNow.Date.AddDays(_config.DaysBeforeExpired);

        if (expires <= until)
        {
            _log.LogWarning($"{domain.DomainName} - Certificate is expired - expieres on: {expires}. Start requesting certificate.");
            return Task.FromResult(Result.Next());
        }

        _log.LogInformation($"{domain.DomainName} - Certificate is valid - expieres on: {expires}");
        return Task.FromResult(Result.Exit());
    }
}
