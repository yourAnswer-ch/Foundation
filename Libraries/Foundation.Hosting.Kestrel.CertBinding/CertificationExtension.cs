using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace Foundation.Hosting.Kestrel.CertBinding;

public static partial class CertificationExtension
{
    [GeneratedRegex("(DNS Name=|DNS:)(?<host>(?=.{1,253}$)(?:(?!-)[A-Za-z0-9-*]{1,63}(?<!-)\\.?)+)")]
    private static partial Regex HostPattern();
    
    public static string[] GetSubjectAlternativeNames(this X509Certificate2 cert)
    {
        var extension = cert.Extensions.FirstOrDefault(e => e.Oid?.Value == "2.5.29.17");
        if (extension == null)
            return Array.Empty<string>();

        var asndata = new AsnEncodedData(extension.Oid, extension.RawData);
        var match = HostPattern().Matches(asndata.Format(false));

        return match.Select(e => e.Groups["host"].Value).ToArray();
    }
}