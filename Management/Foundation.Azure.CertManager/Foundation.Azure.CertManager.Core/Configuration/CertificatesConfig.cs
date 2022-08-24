namespace Foundation.Azure.CertManager.Core.Configuration;

public class CertificatesConfig
{
    public CertificatesConfig()
    {
        AdTenant = new AdTenantConfig();
        LetsEncrypt = new LetsEncryptConfig();
        Certificates = new List<CertificateConfig>();
    }

    public int DaysBeforeExpired { get; set; } = 30;

    public string? FrontDoorName { get; set; }

    public CsrInfoConfig? CsrInfo { get; set; }

    public AdTenantConfig AdTenant { get; private set; }
    
    public LetsEncryptConfig LetsEncrypt { get; private set; }
   
    public List<CertificateConfig> Certificates { get; private set; }
}