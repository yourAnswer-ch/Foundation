﻿using Azure;
using Azure.Identity;
using Azure.ResourceManager.Dns;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager;
using Foundation.Azure.CertManager.Core.Configuration;
using Foundation.Processing.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Foundation.Azure.CertManager.Core.Steps;

public class AzureRemoveTxtRecord : Command
{
    private readonly ILogger _log;
    private readonly CertificatesConfig _config;

    public AzureRemoveTxtRecord(IConfiguration config, ILogger<AzureRemoveTxtRecord> log)
    {
        _log = log;
        _config = config.GetCertManagerConfig();
    }

    public async Task<Result> ExecuteAsync(CertificateConfig domain)
    {
        _log.LogInformation($"{domain.DomainName} - Azure remove txt record.");

        var armClient = new ArmClient(
            new DefaultAzureCredential(
                new DefaultAzureCredentialOptions
                {
                    TenantId = _config.AdTenant.TenantId
                }));

        var subscription = await armClient.GetDefaultSubscriptionAsync();

        ResourceGroupResource resourceGroup = await subscription.GetResourceGroupAsync(domain.ResourceGroup);
        DnsZoneResource zone = await resourceGroup.GetDnsZoneAsync(domain.DomainName);

        var records = zone.GetDnsTxtRecords();
        if (await records.ExistsAsync("_acme-challenge"))
        {
            var current = await records.GetAsync("_acme-challenge");
            await current.Value.DeleteAsync(WaitUntil.Completed);
        }

        return Result.Next();
    }
}