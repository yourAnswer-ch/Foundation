using Microsoft.AspNetCore.Connections;
using System.Security.Cryptography.X509Certificates;

namespace Foundation.Hosting.Kestrel.CertBinding;

internal class CertSelector
{
    private CertificationStore _store;

    public CertSelector(CertificationStore store)
    {
        _store = store;
    }

    public X509Certificate2 SelectCert(ConnectionContext context, string name)
    {
        return _store.GetCertificat(name);
    }
}