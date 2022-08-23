using Certes;
using Certes.Acme;
using Microsoft.Azure.Management.Fluent;

namespace Foundation.Azure.CertManager.Core;

public class Context
{
    public Context()
    {
        DnsTokens = new List<string>();
        Challenges = new List<IChallengeContext>();
        Exceptions = new List<Exception>();
    }

    public AcmeContext? AcmeContext { get; set; }

    public IAccountContext? Account { get; set; }

    public List<IChallengeContext> Challenges { get; } 

    public IOrderContext? Order { get; set; }

    public byte[]? Certificate { get; set; }    

    public List<string> DnsTokens { get; }

    public string? HttpToken { get; set; }

    public string? HttpKey { get; set; }

    public List<Exception> Exceptions { get; }

    public bool Errors => Exceptions.Count > 0;
}