using Certes;
using Foundation.Processing.Pipeline;

namespace Foundation.Azure.CertManager.Core.Steps;

public class LetsEncryptAuthorizeDns : Command
{
    public async Task<Result> ExecuteAsync(Context context)
    {
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
