using Azure.Security.KeyVault.Certificates;
using Foundation.Processing.Pipeline;
using Microsoft.Azure.Management.FrontDoor;
using Microsoft.Azure.Management.FrontDoor.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;


namespace Foundation.Azure.CertManager.Core.Steps;


public class AzureFrontDoor : Command
{
    CertificateClient _client;

    public AzureFrontDoor(CertificateClient client)
    {
        _client = client;
    }

    public async Task<Result> ExecuteAsync(Context context, string resourceGroup, string frontDoorName, string domain)
    {
        //var saveDomainName = domain.Replace('.', '-');

        ////var versions = await _client.GetCertificateVersionsWithHttpMessagesAsync(context.Config.CertManager.KeyVault.BaseUrl, $"wildcard-{saveDomainName}");
        //var versions = _client.GetPropertiesOfCertificateVersions($"wildcard-{saveDomainName}");
        //var version = versions.OrderByDescending(e => e.CreatedOn).FirstOrDefault();

        //if (version == null)
        //    throw new ArgumentException("Certification version not found.");

        //var sp = new ServicePrincipalLoginInformation
        //{
        //    ClientId = context.Config.Azure.ClientId,
        //    ClientSecret = context.Config.Azure.ClientSecret
        //};
        //var credentials = new AzureCredentials(sp, context.Config.Azure.TenantId, AzureEnvironment.AzureGlobalCloud);
        //var client = new FrontDoorManagementClient(credentials)
        //{
        //    SubscriptionId = "8e3e349f-900b-40bc-9916-ec90ca6b2792"
        //};

        ////KeyVault ResourceID
        //var id = "/subscriptions/8e3e349f-900b-40bc-9916-ec90ca6b2792/resourceGroups/RG-Infrastructure/providers/Microsoft.KeyVault/vaults/KV-FD-Certificates";

        //var https = new CustomHttpsConfiguration
        //{
        //    CertificateSource = "AzureKeyVault",
        //    Vault = new KeyVaultCertificateSourceParametersVault(id: id),
        //    CertificateType = "Dedicated",
        //    MinimumTlsVersion = "1.2",
        //    SecretName = version.Name,
        //    SecretVersion = version.Version
        //};

        //await client.FrontendEndpoints.BeginEnableHttpsAsync(resourceGroup, frontDoorName, saveDomainName, https);

        //await client.FrontendEndpoints.BeginEnableHttpsAsync(resourceGroup, frontDoorName, $"www-{saveDomainName}", https);

        return Result.Next();
    }
}

//internal static class FrontDoorSteps
//{
//    public static async Task UpdateCertificate(this Context context, string resourceGroup, string frontDoorName, string domain)
//    {
//        await Execute(context, "", true, async c =>
//         {
//             var saveDomainName = domain.Replace('.', '-');

//             var versions = await context.KeyVaultClient.GetCertificateVersionsWithHttpMessagesAsync(context.Config.CertManager.KeyVault.BaseUrl, $"wildcard-{saveDomainName}");
//             var version = versions.Body.OrderByDescending(e => e.Attributes.Created).FirstOrDefault();

//             if (version == null)
//                 throw new ArgumentException("Certification version not found.");

//             var sp = new ServicePrincipalLoginInformation
//             {
//                 ClientId = context.Config.Azure.ClientId,
//                 ClientSecret = context.Config.Azure.ClientSecret
//             };
//             var credentials = new AzureCredentials(sp, context.Config.Azure.TenantId, AzureEnvironment.AzureGlobalCloud);
//             var client = new FrontDoorManagementClient(credentials)
//             {
//                 SubscriptionId = "8e3e349f-900b-40bc-9916-ec90ca6b2792"
//             };

//            //KeyVault ResourceID
//            var id = "/subscriptions/8e3e349f-900b-40bc-9916-ec90ca6b2792/resourceGroups/RG-Infrastructure/providers/Microsoft.KeyVault/vaults/KV-FD-Certificates";

//             var https = new CustomHttpsConfiguration
//             {
//                 CertificateSource = "AzureKeyVault",
//                 Vault = new KeyVaultCertificateSourceParametersVault(id: id),
//                 CertificateType = "Dedicated",                    
//                 MinimumTlsVersion = "1.2",
//                 SecretName = version.Identifier.Name,
//                 SecretVersion = version.Identifier.Version
//             };

//             await client.FrontendEndpoints.BeginEnableHttpsAsync(resourceGroup, frontDoorName, saveDomainName, https);

//             await client.FrontendEndpoints.BeginEnableHttpsAsync(resourceGroup, frontDoorName, $"www-{ saveDomainName}", https);
//         });
//    }

//    private static async Task Execute(Context context, string message, bool skipOnError, Func<Context, Task> method)
//    {
//        var watch = Stopwatch.StartNew();
//        var log = context.Log;
//        try
//        {
//            if (skipOnError && context.Errors)
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
