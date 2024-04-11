using Azure;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Dns;
using Azure.ResourceManager.Dns.Models;
using Azure.ResourceManager.Resources;
using Foundation.Azure.CertManager.Core.Configuration;
using Foundation.Processing.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Foundation.Azure.CertManager.Core.Steps;

public class AzureCreateTxtRecord : Command // AzureManagement
{
    private readonly ILogger _log;
    private readonly CertificatesConfig _config;

    public AzureCreateTxtRecord(IConfiguration config, ILogger<AzureCreateTxtRecord> log) //: base(config)
    {     
        _log = log;
        _config = config.GetCertManagerConfig();
    }


    public async Task<Result> ExecuteAsync(Context context, CertificateConfig domain)
    {
        _log.LogInformation($"{domain.DomainName} - Azure create txt record for validation.");

        var armClient = new ArmClient(
            new DefaultAzureCredential(
                new DefaultAzureCredentialOptions { 
                    TenantId = _config.AdTenant.TenantId
                } ));

        var subscription = await armClient.GetDefaultSubscriptionAsync();

        ResourceGroupResource resourceGroup = await subscription.GetResourceGroupAsync(domain.ResourceGroup);
        if (resourceGroup == null)
            throw new ArgumentException($"Resource group not found - Name: {domain.ResourceGroup}");

        DnsZoneResource zone = await resourceGroup.GetDnsZoneAsync(domain.DomainName);
        if (zone == null)
            throw new ArgumentException($"DNS zone not found - Domain: {domain.DomainName} - ResourceGroup: {domain.ResourceGroup}");

        var records = zone.GetDnsTxtRecords();


        var newRecord = new DnsTxtRecordData() { TtlInSeconds = 3600 };
        foreach (var token in context.DnsTokens)
        {
            _log.LogInformation($"{domain.DomainName} - Add token: {token}");
            newRecord.DnsTxtRecords.Add(new DnsTxtRecordInfo() { Values = { token } });
        }

        await records.CreateOrUpdateAsync(WaitUntil.Completed, "_acme-challenge", newRecord);

        return Result.Next();
    }
}
