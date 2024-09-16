using Azure.Identity;
using Foundation.Services.ImageProcessor.Core.Filters.Images;
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

public enum OutputFormat
{
    Same,
    Jpeg,
    Png,
    WebP
}

public class ImageFilterConfiguration
{
    public string MimeTypes { get; set; } = "*";

    public ScaleMode DefaultMode { get; set; } = ScaleMode.Box;

    public Size DefaultSize { get; set; } = new Size();
    
}
