namespace Foundation.Azure.CertManager.Core.Configuration;

public class CertificatesConfig
{

    public int DaysBeforeExpired { get; set; } = 30;

    public string? FrontDoorName { get; set; }

    public CsrInfoConfig? CsrInfo { get; set; }

    public AdTenantConfig AdTenant { get; private set; } = new AdTenantConfig();
    
    public LetsEncryptConfig LetsEncrypt { get; private set; } = new LetsEncryptConfig();

    public List<CertificateConfig> Certificates { get; private set; } = [];

    public KeyVaultConfig KeyVault { get; private set; } = new KeyVaultConfig();
}
