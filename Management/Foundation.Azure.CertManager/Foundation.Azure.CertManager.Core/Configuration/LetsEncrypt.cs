using Certes.Acme;

namespace Foundation.Azure.CertManager.Core.Configuration;

public class LetsEncrypt
{
    public Account Account { get; set; }

    public Uri ServiceUrl => WellKnownServers.LetsEncryptV2;

    public LetsEncrypt()
    {
        Account = new Account();
    }
}
