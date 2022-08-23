using Certes;
using Foundation.Azure.CertManager.Core.Configuration;
using Foundation.Processing.Pipeline;

namespace Foundation.Azure.CertManager.Core.Steps;

public class LetsEncryptDownloadCert : Command
{
    public async Task<Result> ExecuteAsync(Context context, Domain domain)
    {
        if (string.IsNullOrWhiteSpace(domain.Name))
            throw new ArgumentException("Domain name can not be null;");

        await Task.Delay(TimeSpan.FromSeconds(60));

        var privateKey = KeyFactory.NewKey(KeyAlgorithm.RS256);
        var cert = await context.Order.Generate(new CsrInfo
        {
            CountryName = "CH",
            State = "Bern",
            Locality = "Bern",
            Organization = "FlairFindr AG",
            OrganizationUnit = "Dev",
            CommonName = $"*.{domain.Name}"
        }, privateKey);

        var name = domain.Name.Replace('.', '-');
        var pfxBuilder = cert.ToPfx(privateKey);
        context.Certificate = pfxBuilder.Build(name, "");

        return Result.Next();
    }
}
