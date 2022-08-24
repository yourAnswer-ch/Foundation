using Foundation.Azure.CertManager.Core.Configuration;
using Foundation.Processing.Pipeline;
using Microsoft.Extensions.Logging;

namespace Foundation.Azure.CertManager.Core.Steps;

public class LetsEncryptCreateOrder : Command
{
    private readonly ILogger _log;

    public LetsEncryptCreateOrder(ILogger<LetsEncryptCreateOrder> log)
    {
        _log = log;
    }

    public async Task<Result> ExecuteAsync(Context context, CertificateConfig domain)
    {
        _log.LogInformation($"{domain.DomainName} - Create lets encript order.");

        if (string.IsNullOrWhiteSpace(domain.DomainName))
            throw new ArgumentException("Domain name can not be null.");

        if (context.AcmeContext == null)
            throw new ArgumentException("Acme context is null");

        var identifiers = domain.WildcardCertificate 
            ? new[] { $"*.{domain.DomainName}", domain.DomainName }
            : new[] { domain.DomainName };

        context.Order = await context.AcmeContext.NewOrder(identifiers);

        return Result.Next();
    }
}
