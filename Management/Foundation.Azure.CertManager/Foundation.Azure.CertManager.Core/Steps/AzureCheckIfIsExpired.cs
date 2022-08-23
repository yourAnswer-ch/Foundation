using Azure.Security.KeyVault.Certificates;
using Foundation.Azure.CertManager.Core.Configuration;
using Foundation.Processing.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Foundation.Azure.CertManager.Core.Steps;

public class AzureCheckIfIsExpired : Command
{
    private readonly CertificateClient _client;
    private readonly CertificatesConfig _config;
    private readonly ILogger _log;

    public AzureCheckIfIsExpired(CertificateClient client, IConfiguration config, ILogger<AzureCheckIfIsExpired> log)
    {
        _log = log;
        _client = client;
        _config = config.GetCertManagerConfig();
    }

    public Task<Result> ExecuteAsync(Domain domain)
    {
        if (string.IsNullOrWhiteSpace(domain.Name))
            throw new ArgumentException("Domain name can not be null.");

        var name = $"wildcard-{domain.Name.Replace('.', '-')}";

        var versions = _client.GetPropertiesOfCertificateVersions(name);
        var version = versions.OrderByDescending(e => e.CreatedOn).FirstOrDefault();

        if (version == null)
        {
            _log.LogWarning($"Domain: {domain} version not found. Certificate is considered expired.");
            return Task.FromResult(Result.Next());
        }

        if (version.ExpiresOn == null)
        {
            _log.LogWarning($"Domain: {domain} expires attritute not found. Certificate is considered expired.");
            return Task.FromResult(Result.Next());
        }

        var expires = version.ExpiresOn;
        var until = DateTime.UtcNow.Date.AddDays(_config.DaysBeforeExpired);

        var success = expires <= until;

        if (success)
        {
            _log.LogWarning($"Domain: {domain} is expired - valid till: {expires} - date: {until}");
            return Task.FromResult(Result.Next());
        }
        else
        {
            _log.LogInformation($"Domain: {domain} not expired - valid till: {expires} - date: {until}");
            return Task.FromResult(Result.Exit());
        }        
    }
}




//internal static class AzureSteps
//{
//    public static async Task<bool> IsExpired(this Context context, string domain)
//    {
//        var name = $"wildcard-{domain.Replace('.', '-')}";

//        var versions = await context.KeyVaultClient.GetCertificateVersionsWithHttpMessagesAsync(context.Config.CertManager.KeyVault.BaseUrl, name);
//        var version = versions.Body.OrderByDescending(e => e.Attributes.Created).FirstOrDefault();

//        if (version == null)
//        {
//            context.Log.LogWarning($"Domain: {domain} version not found. Certificate is considered expired.");
//            return true;
//        }

//        if (version.Attributes.Expires == null)
//        {
//            context.Log.LogWarning($"Domain: {domain} expires attritute not found. Certificate is considered expired.");
//            return true;
//        }

//        var expires = version.Attributes.Expires;
//        var until = DateTime.UtcNow.Date.AddDays(context.Config.CertManager.DaysBeforeExpired);

//        var success = expires <= until;

//        if(success)
//        {
//            context.Log.LogWarning($"Domain: {domain} is expired - valid till: {expires} - date: {until}");
//        }
//        else 
//        {
//            context.Log.LogInformation($"Domain: {domain} not expired - valid till: {expires} - date: {until}");
//        }

//        return success;
//    }

//    public static async Task RemoveTxtRecord(this Context context, string resourceGroup, string domain)
//    {
//        await Execute(context, "remove txt record", false, async c => 
//        {
//            await c.AzureClient.DnsZones
//                .GetByResourceGroup(resourceGroup, domain)
//                .Update()
//                .WithoutTxtRecordSet("_acme-challenge")
//                .ApplyAsync();
//        });
//    }

//    public static async Task CreateTxtRecord(this Context context, string resourceGroup, string domain)
//    {
//        await Execute(context, "create txt record", true, async c =>
//        {
//            var record = c.AzureClient.DnsZones
//                .GetByResourceGroup(resourceGroup, domain)
//                .Update()
//                .DefineTxtRecordSet("_acme-challenge");

//            IWithTxtRecordTextValueOrAttachable<IUpdate> update = null;
//            foreach (var token in c.DnsTokens)
//            {
//                update = record.WithText(token);
//            }

//            if (update != null)
//            {
//                await update.Attach().ApplyAsync();
//            }
//        });
//    }

//    public static async Task StoreCert(this Context context, string domain)
//    {
//        await Execute(context, "store certificate", true, async c =>
//        {
//            var name = $"wildcard-{domain.Replace('.', '-')}";
//            var cert = Convert.ToBase64String(c.Certificate);
//            await c.KeyVaultClient.ImportCertificateWithHttpMessagesAsync(c.Config.CertManager.KeyVault.BaseUrl, name, cert);
//        });
//    }

//    public static async Task GetKeyVaultClient(this Context context)
//    {
//        await Execute(context, "create key vault client", true, c =>
//        {
//            var azureServiceTokenProvider = new AzureServiceTokenProvider();
//            context.KeyVaultClient = new KeyVaultClient(
//                new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

//            return Task.CompletedTask;
//        });
//    }

//    public static async Task CreateAzureClient(this Context context)
//    {
//        await Execute(context, "create azure client", true, c =>
//        {
//            var sp = new ServicePrincipalLoginInformation
//            {
//                ClientId = c.Config.Azure.ClientId,
//                ClientSecret = c.Config.Azure.ClientSecret
//            };
//            var credentials = new AzureCredentials(sp, c.Config.Azure.TenantId, AzureEnvironment.AzureGlobalCloud);

//            var authenticatedAzure = Microsoft.Azure.Management.Fluent.Azure
//                .Configure()
//                .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
//                .Authenticate(credentials);

//            c.AzureClient = authenticatedAzure.WithDefaultSubscription();

//            return Task.CompletedTask;
//        });
//    }

//    private static async Task Execute(Context context, string message, bool skipOnError, Func<Context, Task> method)
//    {
//        var watch = Stopwatch.StartNew();
//        var log = context.Log;
//        try
//        {
//            if(skipOnError && context.Errors)
//            {
//                log.LogWarning($"Skip: {message}");
//                return;
//            }

//            log.LogInformation($"Start: {message}");
//            await method.Invoke(context);
//        }
//        catch (Exception ex)
//        {
//            context.Exceptions.Add(ex);
//            log.LogError($"Error: {message}{Environment.NewLine}{ex}");
//        }
//        finally
//        {
//            watch.Stop();
//            log.LogInformation($"Done: {message} - Duration: {watch.ElapsedMilliseconds:N0}ms");
//        }
//    }
//}
