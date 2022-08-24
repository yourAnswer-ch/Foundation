using Foundation.Azure.CertManager.Core.Configuration;
using Foundation.Processing.Pipeline;
using Microsoft.Extensions.Logging;

namespace Foundation.Azure.CertManager.Core.Steps;

public class LetsEncryptValidate : Command
{
    private readonly ILogger _log;

    public LetsEncryptValidate(ILogger<LetsEncryptValidate> log)
    {
        _log = log;
    }

    public async Task<Result> ExecuteAsync(Context context, CertificateConfig domain)
    {
        _log.LogInformation($"{domain.DomainName} - Validate lets encript order.");

        foreach (var challenge in context.Challenges)
        {
            var v = await challenge.Validate();
            _log.LogInformation($"{domain.DomainName} - Challenge type: {v.Type} status: {v.Status}");
        }

        return Result.Next();
    }
}
