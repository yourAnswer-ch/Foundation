using Certes;
using Foundation.Azure.CertManager.Core.Configuration;
using Foundation.Processing.Pipeline;
using Microsoft.Extensions.Configuration;

namespace Foundation.Azure.CertManager.Core.Steps;

public class LetsEncryptCreateAccount : Command
{
    private readonly CertificatesConfig _config;

    public LetsEncryptCreateAccount(IConfiguration config)
    {
        _config = config.GetCertManagerConfig();
    }

    public async Task<Result> ExecuteAsync(Context context)
    {
        context.AcmeContext = new AcmeContext(_config.LetsEncrypt.ServiceUrl);
        context.Account = await context.AcmeContext.NewAccount(_config.LetsEncrypt.Account.EMail, true);

        return Result.Next();
    }
}
