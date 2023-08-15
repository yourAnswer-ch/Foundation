namespace Foundation.Hosting.Kestrel.CertBinding.Configuration;

internal class KestrelBindingConfig
{
    public string IPAddress { get; set; } = "*";

    public int Port { get; set; } = 443;

    public string Protocols { get; set; } = "HTTP2|HTTP3";

    public CertificateConfig? Certificate { get; set; }
}
