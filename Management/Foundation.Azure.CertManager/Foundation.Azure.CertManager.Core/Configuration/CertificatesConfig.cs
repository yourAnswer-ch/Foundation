namespace Foundation.Azure.CertManager.Core.Configuration;

public class CertificatesConfig
{
    public CertificatesConfig()
    {
        AdTenant = new AdTenantConfig();
        LetsEncrypt = new LetsEncrypt();
        Domains = new List<Domain>();
    }

    public int DaysBeforeExpired { get; set; } = 30;

    public string? FrontDoorName { get; set; } 

    public AdTenantConfig AdTenant { get; private set; }
    
    public LetsEncrypt LetsEncrypt { get; private set; }
  
    public List<Domain> Domains { get; private set; }
}