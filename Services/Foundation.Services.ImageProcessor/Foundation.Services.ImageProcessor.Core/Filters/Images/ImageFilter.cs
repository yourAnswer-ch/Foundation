using ImageMagick;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace Foundation.Services.ImageProcessor.Core.Filters.Images;

public partial class ImageFilter : IFilter
{
    [GeneratedRegex(@"\A(?<Width>\d+)x(?<Height>\d+)|(?<Size>\d+)\z")]
    private static partial Regex GetSizePattern();

    public async Task Filter(HttpContext context, Stream stream, string mimeType)
    {
        var parameters = ParseParameters(context.Request.Query);

        switch (parameters.Item1)
        {
            case ScaleMode.Box:
                var image = ScaleBox(stream, parameters.Item2);
                await WriteImage(context, image, mimeType);
                break;

            case ScaleMode.Fit:
                image = ScaleFit(stream, parameters.Item2);
                await WriteImage(context, image, mimeType);
                break;

            case ScaleMode.Exact:
                image = ScaleExact(stream, parameters.Item2);
                await WriteImage(context, image, mimeType);
                break;

            case ScaleMode.Original:
                context.Response.ContentType = mimeType;
                await stream.CopyToAsync(context.Response.Body);
                break;
        }
    }

    private async Task WriteImage(HttpContext context, MagickImage image, string mimeType)    
    {        
        // code may can be improoved
        if (context.Request.Headers.TryGetValue("Accept", out var accept))
        {
            var result = StringWithQualityHeaderValue.ParseList(accept);
            if (result.Any(e => e.Value == "webp"))
            {
                context.Response.ContentType = "image/webp";
                await image.WriteAsync(context.Response.Body, MagickFormat.WebP);
                return;
            }
        }
        
        context.Response.ContentType = mimeType;        
        var format = GetMagickFormatFromMimeType(mimeType);
        await image.WriteAsync(context.Response.Body, format);
    }

    private MagickImage ScaleFit(Stream stream, Size dimension)
    {
        var image = new MagickImage(stream);
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
        return image;
    }

    private MagickImage ScaleBox(Stream stream, Size dimension)
    {
        var image = new MagickImage(stream);
        var box = new MagickGeometry(dimension.Width, dimension.Height)
        {
            FillArea = false,
            IgnoreAspectRatio = false,
        };

        image.Resize(box);
        return image;
    }

    private MagickImage ScaleExact(Stream stream, Size dimension)
    {
        var image = new MagickImage(stream);
        var box = new MagickGeometry(dimension.Width, dimension.Height)
        {
            IgnoreAspectRatio = true,
        };

        image.Resize(box);
        return image;
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

        return (ScaleMode.Original, new Size(0, 0));
    }

    private Size ParseSize(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return new Size(600, 600);

        var match = GetSizePattern().Match(value);

        if (!match.Success)
            throw new ArgumentException("Invalid size");

        if (match.Groups.TryGetValue("Size", out var group) && int.TryParse(group.Value, out int size))
            return new Size(size, size);

        if (match.Groups.TryGetValue("Width", out var groupWidth) &&
           match.Groups.TryGetValue("Height", out var groupHeight) &&
           int.TryParse(groupWidth.Value, out int width) &&
           int.TryParse(groupHeight.Value, out int height))
            return new Size(width, height);

        throw new ArgumentException("Invalid size");
    }

    static MagickFormat GetMagickFormatFromMimeType(string mimeType)
    {
        var format = MagickNET.SupportedFormats.FirstOrDefault(f => string.Equals(f.MimeType, mimeType, StringComparison.OrdinalIgnoreCase));
        return format != null 
            ? format.Format 
            : throw new ArgumentException($"Unknown MIME type: {mimeType}");
    }
}
