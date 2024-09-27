using Foundation.Services.ImageProcessor.Core.Filters.Images;

namespace Foundation.Services.ImageProcessor.Core.Configuration;

public class ImageFilterConfiguration
{
    public string MimeTypes { get; set; } = "*";

    public ScaleMode DefaultMode { get; set; } = ScaleMode.Box;

    public Size DefaultSize { get; set; } = new Size();
    
}
