namespace Foundation.Services.ImageProcessor.Core.Filters.Images;

public class Size
{
    public Size() { }

    public Size(uint width, uint hight)
    {
        Width = width;
        Height = hight;
    }

    public uint Width { get; set; } = 1024;

    public uint Height { get; set; } = 768;
}