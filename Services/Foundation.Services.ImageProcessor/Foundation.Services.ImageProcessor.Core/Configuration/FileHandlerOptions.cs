namespace Foundation.Services.ImageProcessor.Core.Configuration;

public class FileHandlerOptions
{
    public string Path { get; set; } = "files";
    
    public string Container { get; set; } = "profiles";
    
    public required Uri StorageUrl { get; set; }

    public string? CorsPolicy { get; set; }
   
    public ImageFilterConfiguration? ImageFilter { get; set; }
}
