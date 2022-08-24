using Azure.Security.KeyVault.Certificates;
using Foundation.Azure.CertManager.Core.Configuration;
using Foundation.Processing.Pipeline;
using Microsoft.Extensions.Logging;

namespace Foundation.Azure.CertManager.Core.Steps;

public class AzureStoreCert : Command
{
    private readonly ILogger _log;
    private readonly CertificateClient _client;

    public AzureStoreCert(CertificateClient client, ILogger<AzureStoreCert> log)
    {
        _log = log;
        _client = client;
    }

    public async Task<Result> ExecuteAsync(Context context, CertificateConfig domain)
    {
        _log.LogInformation($"{domain.DomainName} - Azure store certificate in key vault");

        if (string.IsNullOrWhiteSpace(domain.DomainName))
            throw new ArgumentException("Domain name can not be null.");

        await _client.ImportCertificateAsync(new ImportCertificateOptions(domain.CertificatName, context.Certificate));

        return Result.Next();
    }
}