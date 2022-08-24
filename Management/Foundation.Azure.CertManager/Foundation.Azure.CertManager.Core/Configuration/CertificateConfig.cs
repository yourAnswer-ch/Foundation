namespace Foundation.Azure.CertManager.Core.Configuration;

public class CertificateConfig
{
    public string? DomainName { get; set; }

    public string? CertificatName => !string.IsNullOrEmpty(DomainName) ? $"wildcard-{DomainName?.Replace('.', '-')}" : null;

    public string? ResourceGroup { get; set; }

    public bool WildcardCertificate { get; set; } = true;

    public CsrInfoConfig? CsrInfo { get; set; }
}