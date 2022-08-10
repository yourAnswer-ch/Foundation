namespace Foundation.Hosting.Kestrel.CertBinding.Configuration;

internal class KestrelConfig
{
    public bool AddServerHeader { get; set; } = false;

    public List<KestrelBindingConfig> Bindings { get; set; } = new List<KestrelBindingConfig>();
}
