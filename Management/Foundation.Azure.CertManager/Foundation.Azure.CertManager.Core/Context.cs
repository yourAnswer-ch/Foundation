using Certes;
using Certes.Acme;

namespace Foundation.Azure.CertManager.Core;

public class Context
{
    public Context()
    {
        DnsTokens = new List<string>();
        Challenges = new List<IChallengeContext>();
    }

    public AcmeContext? AcmeContext { get; set; }

    public List<IChallengeContext> Challenges { get; } 

    public IOrderContext? Order { get; set; }

    public byte[]? Certificate { get; set; }    

    public List<string> DnsTokens { get; }

    public bool IsValid { get; set; }
}