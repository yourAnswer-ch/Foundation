namespace Foundation.Services.ImageProcessor.Core.Filters.Images;

public class Size
{
    public Size() { }

    public Size(int width, int hight)
    {
        Width = width;
        Height = hight;
    }

    public int Width { get; set; } = 1024;

    public int Height { get; set; } = 768;
}