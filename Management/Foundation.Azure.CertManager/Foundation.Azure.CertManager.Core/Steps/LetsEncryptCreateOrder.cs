using Foundation.Azure.CertManager.Core.Configuration;
using Foundation.Processing.Pipeline;

namespace Foundation.Azure.CertManager.Core.Steps;

public class LetsEncryptCreateOrder : Command
{
    public async Task<Result> ExecuteAsync(Context context, Domain domain)
    {
        if (string.IsNullOrWhiteSpace(domain.Name))
            throw new ArgumentException("Domain name can not be null.");

        if (context.AcmeContext == null)
            throw new ArgumentException("Acme context is null");

        context.Order = await context.AcmeContext.NewOrder(new[] { $"*.{domain.Name}", domain.Name });

        return Result.Next();
    }
}
