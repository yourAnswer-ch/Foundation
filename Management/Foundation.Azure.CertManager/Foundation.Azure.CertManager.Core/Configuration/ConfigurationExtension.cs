using Microsoft.Extensions.Configuration;

namespace Foundation.Azure.CertManager.Core.Configuration;

public static  class ConfigurationExtension
{
    public static CertificatesConfig GetCertManagerConfig(this IConfiguration config, string key = "Azure:Certificates")
    {
        return config.GetSection(key).Get<CertificatesConfig>();
    }
}
