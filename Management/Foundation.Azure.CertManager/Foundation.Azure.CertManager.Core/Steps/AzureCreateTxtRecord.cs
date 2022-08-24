using Foundation.Azure.CertManager.Core.Configuration;
using Foundation.Processing.Pipeline;
using Microsoft.Azure.Management.Dns.Fluent.DnsRecordSet.UpdateDefinition;
using Microsoft.Azure.Management.Dns.Fluent.DnsZone.Update;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Foundation.Azure.CertManager.Core.Steps;

public class AzureCreateTxtRecord : AzureManagement
{
    private readonly ILogger _log;

    public AzureCreateTxtRecord(IConfiguration config, ILogger<AzureCreateTxtRecord> log) : base(config)
    {     
        _log = log;
    }

    public async Task<Result> ExecuteAsync(Context context, CertificateConfig domain)
    {
        _log.LogInformation($"{domain.DomainName} - Azure create txt record for validation.");

        var record = CreateClient().DnsZones
            .GetByResourceGroup(domain.ResourceGroup, domain.DomainName)
            .Update()
            .DefineTxtRecordSet("_acme-challenge");

        IWithTxtRecordTextValueOrAttachable<IUpdate>? update = null;
        foreach (var token in context.DnsTokens)
        {
            update = record.WithText(token);
        }

        if (update != null)
        {
            await update.Attach().ApplyAsync();
        }

        return Result.Next();
    }
}
