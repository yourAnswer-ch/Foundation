using Microsoft.Extensions.Configuration;

namespace Foundation.Hosting.Info.Configuration;

public static class ConfigExtensions
{
    public static InfoConfig GetInfoConfig(this IConfiguration config, string section = "ApiStatus")
    {
        return config.GetSection(section).Get<InfoConfig>() ?? new InfoConfig();
    }
}
