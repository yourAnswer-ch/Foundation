using Certes;
using Foundation.Azure.CertManager.Core.Configuration;
using Foundation.Processing.Pipeline;
using Microsoft.Extensions.Logging;

namespace Foundation.Azure.CertManager.Core.Steps;

public class LetsEncryptAuthorizeDns : Command
{
    private readonly ILogger _log;

    public LetsEncryptAuthorizeDns(ILogger<LetsEncryptAuthorizeDns> log)
    {
        _log = log;
    }

    public async Task<Result> ExecuteAsync(Context context, CertificateConfig domain)
    {
        _log.LogInformation($"{domain.DomainName} - Create lets encript authorize dns.");

        if (context.Order == null)
            throw new ArgumentException("Order can not be null.");

        if (context.AcmeContext == null)
            throw new ArgumentException("Acme context is null");

        var authorizations = await context.Order.Authorizations();

        foreach (var authorization in authorizations)
        {
            var challenge = await authorization.Dns();
            context.Challenges.Add(challenge);

            var token = context.AcmeContext.AccountKey.DnsTxt(challenge.Token);
            context.DnsTokens.Add(token);
        }

        return Result.Next();
    }
}
