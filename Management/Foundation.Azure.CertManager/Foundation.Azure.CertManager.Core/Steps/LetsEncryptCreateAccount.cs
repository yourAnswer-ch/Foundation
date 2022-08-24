using Certes;
using Foundation.Azure.CertManager.Core.Configuration;
using Foundation.Processing.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Foundation.Azure.CertManager.Core.Steps;

public class LetsEncryptCreateAccount : Command
{
    private readonly ILogger _log;
    private readonly CertificatesConfig _config;

    public LetsEncryptCreateAccount(IConfiguration config, ILogger<AzureCheckIfIsExpired> log)
    {
        _log = log;
        _config = config.GetCertManagerConfig();
    }

    public async Task<Result> ExecuteAsync(Context context, CertificateConfig domain)
    {
        _log.LogInformation($"{domain.DomainName} - Create lets encript account.");
        context.AcmeContext = new AcmeContext(_config.LetsEncrypt.ServiceUrl);
        context.Account = await context.AcmeContext.NewAccount(_config.LetsEncrypt.Account.EMail, true);

        return Result.Next();
    }
}
