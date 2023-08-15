namespace Foundation.Hosting.Kestrel.CertBinding.Configuration;

internal class CertificateConfig
{
    public string? Source { get; set; }

    public List<string> Names { get; set; }

    public string Protocols { get; set; } = "TLS12|TLS13";

    public CertificateConfig()
    {
        Names = new List<string>();
    }
}
