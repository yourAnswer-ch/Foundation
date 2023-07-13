using Certes;
using Foundation.Azure.CertManager.Core.Configuration;
using Foundation.Processing.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Foundation.Azure.CertManager.Core.Steps;

public class LetsEncryptDownloadCert : Command
{
    private readonly ILogger _log;
    private readonly CertificatesConfig _config;

    public LetsEncryptDownloadCert(IConfiguration config, ILogger<LetsEncryptDownloadCert> log)
    {
        _log = log;
        _config = config.GetCertManagerConfig();
    }

    public async Task<Result> ExecuteAsync(Context context, CertificateConfig domain)
    {
        _log.LogInformation($"{domain.DomainName} - Download lets encript certificate.");

        if (string.IsNullOrWhiteSpace(domain.DomainName))
            throw new ArgumentException("Domain name can not be null;");

        var csrinfo = GetCsrInfo(domain);
        var privateKey = KeyFactory.NewKey(KeyAlgorithm.RS256);
        
        var cert = await context.Order.Generate(csrinfo, privateKey);

        var name = domain.DomainName.Replace('.', '-');
        var pfxBuilder = cert.ToPfx(privateKey);
        context.Certificate = pfxBuilder.Build(name, "");

        return Result.Next();
    }

    private CsrInfo GetCsrInfo(CertificateConfig domain)
    {
        var commonName = domain.WildcardCertificate ? $"*.{domain.DomainName}" : domain.DomainName;

        if (domain.CsrInfo != null)
        {
            return new CsrInfo
            {
                CountryName = domain.CsrInfo.CountryName,
                State = domain.CsrInfo.State,
                Locality = domain.CsrInfo.Locality,
                Organization = domain.CsrInfo.Locality,
                OrganizationUnit = domain.CsrInfo.OrganizationUnit,
                CommonName = commonName
            };
        }

        if (_config.CsrInfo == null)
            throw new ArgumentException("Missing csr info config.");

        return new CsrInfo
        {
            CountryName = _config.CsrInfo.CountryName,
            State = _config.CsrInfo.State,
            Locality = _config.CsrInfo.Locality,
            Organization = _config.CsrInfo.Locality,
            OrganizationUnit = _config.CsrInfo.OrganizationUnit,
            CommonName = commonName
        };
    }
}
