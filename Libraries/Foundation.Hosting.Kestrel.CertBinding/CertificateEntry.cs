using System.Security.Cryptography.X509Certificates;

namespace Foundation.Hosting.Kestrel.CertBinding;

internal class CertificateEntry
{
    public X509Certificate2 Certificate {  get; private set; } 

    public string Name { get; private set; }

    internal CertificateEntry(X509Certificate2 certificate, string name)
    {
        Name = name;
        Certificate = certificate;        
    }

    public bool Matches(string hostname)
    {
        return IsMatch(hostname, Name);
    }

    private bool IsMatch(string sni, string pattern)
    {
        var patternLabels = pattern.Split('.');
        var sniLabels = sni.Split('.');

        if (patternLabels.Length != sniLabels.Length)
            return false;

        for (int i = 0; i < patternLabels.Length; i++)
        {
            if (!IsLabelMatch(patternLabels[i], sniLabels[i]))
                return false;
        }

        return true;
    }

    private bool IsLabelMatch(string patternLabel, string sniLabel)
    {
        if (patternLabel == "*")
            return true;

        return string.Equals(patternLabel, sniLabel, StringComparison.InvariantCultureIgnoreCase);
    }
}