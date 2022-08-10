namespace Foundation.Hosting.Kestrel.CertBinding.Configuration;

internal class CertificateConfig
{
    public string? Source { get; set; }

    public string? Name { get; set; }

    public string Protocols { get; set; } = "TLS12|TLS13";
}
