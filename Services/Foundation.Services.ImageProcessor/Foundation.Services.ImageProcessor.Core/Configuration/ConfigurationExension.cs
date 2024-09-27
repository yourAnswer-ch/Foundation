using Microsoft.Extensions.Configuration;

namespace Foundation.Services.ImageProcessor.Core.Configuration;

public static class ConfigurationExension
{
    public static string? GetBlobStorageConnectionString(this IConfiguration config, string key = "Flowcpt:BlobStorage:Storage:ConnectionString")
    {
        return config.GetSection(key).Get<string>();
    }

    public static FileHandlerConfiguration? GetFileHandlerConfig(this IConfiguration config, string key = "FileHandler")
    {
        return config.GetSection(key).Get<FileHandlerConfiguration>();
    }
}
