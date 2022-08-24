using Foundation.Azure.CertManager.Core.Configuration;
using Foundation.Processing.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Foundation.Azure.CertManager.Core.Steps;

public class AzureRemoveTxtRecord : AzureManagement
{
    private readonly ILogger _log;

    public AzureRemoveTxtRecord(IConfiguration config, ILogger<AzureRemoveTxtRecord> log)
        : base(config) {
        _log = log;
    }

    public async Task<Result> ExecuteAsync(CertificateConfig domain)
    {
        _log.LogInformation($"{domain.DomainName} - Azure remove txt record.");

        await CreateClient().DnsZones
            .GetByResourceGroup(domain.ResourceGroup, domain.DomainName)
            .Update()
            .WithoutTxtRecordSet("_acme-challenge")
            .ApplyAsync();

        return Result.Next();
    }
}