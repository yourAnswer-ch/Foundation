using Certes.Acme;

namespace Foundation.Azure.CertManager.Core.Configuration;

public class LetsEncryptConfig
{
    public AccountConfig Account { get; set; }

    public Uri ServiceUrl => WellKnownServers.LetsEncryptV2;

    public LetsEncryptConfig()
    {
        Account = new AccountConfig();
    }
}
