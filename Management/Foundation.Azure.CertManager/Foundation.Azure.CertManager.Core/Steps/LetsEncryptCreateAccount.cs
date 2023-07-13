using Certes;
using Certes.Acme;
using Foundation.Azure.CertManager.Core.Configuration;
using Foundation.Processing.Pipeline;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Foundation.Azure.CertManager.Core.Steps;

public class LetsEncryptCreateAccount : Command
{
    private readonly ILogger _log;
    private readonly CertificatesConfig _config;
    private readonly IMemoryCache _memoryCache;

    public LetsEncryptCreateAccount(IConfiguration config, ILogger<AzureCheckIfIsExpired> log, IMemoryCache memoryCache)
    {
        _log = log;
        _config = config.GetCertManagerConfig();
        _memoryCache = memoryCache;
    }

    public async Task<Result> ExecuteAsync(Context context, CertificateConfig domain)
    {       
        var service = WellKnownServers.LetsEncryptV2;

        if (_memoryCache.TryGetValue<string>("AccountKey", out var currentKey))
        {
            _log.LogInformation($"{domain.DomainName} - Reconnect Let's Encrypt account.");

            var accountKey = KeyFactory.FromPem(currentKey);
            context.AcmeContext = new AcmeContext(service, accountKey);
        }
        else
        {
            _log.LogInformation($"{domain.DomainName} - Create Let's Encrypt account.");
            
            context.AcmeContext = new AcmeContext(service);
            
            await context.AcmeContext.NewAccount(_config.LetsEncrypt.Account.EMail, true);
            var key = context.AcmeContext.AccountKey.ToPem();

            _memoryCache.Set("AccountKey", key, TimeSpan.FromMinutes(90));
        }
       
        await Task.Delay(TimeSpan.FromSeconds(20));

        return Result.Next();
    }
}
