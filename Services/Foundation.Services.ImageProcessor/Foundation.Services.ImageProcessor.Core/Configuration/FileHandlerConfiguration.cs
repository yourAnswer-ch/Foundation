namespace Foundation.Services.ImageProcessor.Core.Configuration;

public class FileHandlerConfiguration
{
    public string Path { get; set; } = "files";
    
    public string Container { get; set; } = "profiles";

    public string ClientId { get; set; } = "FlowcptStorageAccount";

    public string? CorsPolicy { get; set; }
   
    public ImageFilterConfiguration? ImageFilter { get; set; }
}
