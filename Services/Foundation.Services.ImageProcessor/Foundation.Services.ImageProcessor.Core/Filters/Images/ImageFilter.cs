using ImageMagick;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace Foundation.Services.ImageProcessor.Core.Filters.Images;

public partial class ImageFilter : IFilter
{
    [GeneratedRegex(@"\A(?<Width>\d+)x(?<Height>\d+)|(?<Size>\d+)\z")]
    private static partial Regex GetSizePattern();

    public async Task Filter(HttpContext context, Stream stream)
    {
        var image = new MagickImage(stream);

        var parameters = ParseParameters(context.Request.Query);

        switch(parameters.Item1)
        {
            case ScaleMode.Box:
                ScaleBox(image, parameters.Item2);
                break;

            case ScaleMode.Fit:
                ScaleFit(image, parameters.Item2);
                break;

            case ScaleMode.Exact:
                ScaleExact(image, parameters.Item2);
                break;

            case ScaleMode.Default:
                ScaleBox(image, parameters.Item2);
                break;
        }

        context.Response.ContentType = "image/webp";
        await image.WriteAsync(context.Response.Body, MagickFormat.WebP);
        //context.Response.ContentType = "image/jpeg";
        //await image.WriteAsync(context.Response.Body, MagickFormat.Jpeg);
        context.Response.Body.Close();
    }

    private void ScaleFit(MagickImage image, Size dimension)
    {
        var box = new MagickGeometry(dimension.Width, dimension.Height)
        {
            FillArea = true,
            IgnoreAspectRatio = false,
        };

        image.Resize(box);

        var square = new MagickGeometry(dimension.Width, dimension.Height)
        {
            X = Math.Max(0, (image.Width - dimension.Width) / 2),
            Y = Math.Max(0, (image.Height - dimension.Height) / 2)
        };

        image.Crop(square);
        image.RePage();
    }

    private void ScaleBox(MagickImage image, Size dimension)
    {
        var box = new MagickGeometry(dimension.Width, dimension.Height)
        {
            FillArea = false,
            IgnoreAspectRatio = false,
        };

        image.Resize(box);
    }

    private void ScaleExact(MagickImage image, Size dimension)
    {
        var box = new MagickGeometry(dimension.Width, dimension.Height)
        {            
            IgnoreAspectRatio = true,
        };

        image.Resize(box);
    }

    private (ScaleMode, Size) ParseParameters(IQueryCollection query)
    {
        if (query.Keys.Contains("box"))
        {
            return (ScaleMode.Box, ParseSize(query["box"]));
        }
        else if (query.Keys.Contains("fit"))
        {
            return (ScaleMode.Fit, ParseSize(query["fit"]));
        }
        else if (query.Keys.Contains("exact"))
        {
            return (ScaleMode.Exact, ParseSize(query["exact"]));
        }

        return (ScaleMode.Exact, new Size(600, 600));
    }

    private Size ParseSize(string? value)
    {
        if(string.IsNullOrEmpty(value))
            return new Size(600, 600);  

        var match = GetSizePattern().Match(value);

        if (!match.Success)
            throw new ArgumentException("Invalid size");

        if(match.Groups.TryGetValue("Size", out var group) && int.TryParse(group.Value, out int size))
            return new Size(size, size);

        if(match.Groups.TryGetValue("Width", out var groupWidth) && 
           match.Groups.TryGetValue("Height", out var groupHeight) &&
           int.TryParse(groupWidth.Value, out int width) &&
           int.TryParse(groupHeight.Value, out int height))
            return new Size(width, height);

        throw new ArgumentException("Invalid size");
    }
}
