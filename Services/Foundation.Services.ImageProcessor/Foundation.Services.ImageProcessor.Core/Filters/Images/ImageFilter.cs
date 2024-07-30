using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Foundation.Services.ImageProcessor.Core.Caching;
using ImageMagick;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace Foundation.Services.ImageProcessor.Core.Filters.Images;

public partial class ImageFilter(CacheService cacheService) : IFilter
{
    [GeneratedRegex(@"\A(?<Width>\d+)x(?<Height>\d+)|(?<Size>\d+)\z")]
    private static partial Regex GetSizePattern();

    public string CalculateCacheKey(string sourcePath, IMagickFormatInfo format, ScaleMode mode, Size size)
    {
        var parameters = mode switch
        {
            ScaleMode.Box => $"box-{size.Width}x{size.Height}",
            ScaleMode.Fit => $"fit-{size.Width}x{size.Height}",
            ScaleMode.Exact => $"exact-{size.Width}x{size.Height}",
            _ => "original"
        };

        return $"{sourcePath}/image-{parameters}.{GetFileExtension(format)}";
    }

    public async Task Filter(HttpContext context, BlobClient client, BlobProperties properties)
    {
        var acceptHeader = context.Request.Headers[HeaderNames.Accept];

        var parameters = ParseParameters(context.Request.Query);
        var format = EveluateFileFormat(context, properties.ContentType);
        var eTag = properties.ETag.ToString();

        if (parameters.mode == ScaleMode.Original)
        {
            using (var original = await client.OpenReadAsync())
            {
                await WriteRawImage(context, original, properties.ContentType, eTag);
                return;
            }
        }

        var cachepath = CalculateCacheKey(client.Uri.AbsolutePath, format, parameters.mode, parameters.size);

        if (await cacheService.TryGetFileFromCache(cachepath,
            async (s, c) => await WriteRawImage(context, s, c, eTag)))
            return;

        using (var stream = await client.OpenReadAsync())
        {
            using (var image = TransformImage(stream, parameters.mode, parameters.size))
            {
                await WriteImage(context, image, format, eTag);
                await WriteCach(image, cachepath, format);
            }
        }
    }

    private async Task WriteImage(HttpContext context, MagickImage image, IMagickFormatInfo format, string eTag)
    {
        context.Response.ContentType = format.MimeType!;
        context.Response.Headers.ETag = eTag;
        context.Response.Headers.CacheControl = "public,max-age=604800";
        await image.WriteAsync(context.Response.Body, format.Format);
    }

    private async Task WriteRawImage(HttpContext context, Stream stream, string contentType, string eTag)
    {
        context.Response.ContentType = contentType;
        context.Response.Headers.ETag = eTag;
        context.Response.Headers.CacheControl = "public,max-age=604800";
        await stream.CopyToAsync(context.Response.Body);
    }

    private async Task WriteCach(MagickImage image, string cachepath, IMagickFormatInfo format)
    {
        await cacheService.SaveFileToCache(cachepath, format.MimeType!, async s => await image.WriteAsync(s, format.Format));
    }

    private MagickImage TransformImage(Stream stream, ScaleMode mode, Size size)
    {
        return mode switch
        {
            ScaleMode.Box => ScaleBox(stream, size),
            ScaleMode.Fit => ScaleFit(stream, size),
            ScaleMode.Exact => ScaleExact(stream, size),
            _ => throw new InvalidOperationException("Invalid scale mode"),
        };
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

    private (ScaleMode mode, Size size) ParseParameters(IQueryCollection query)
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

    private IMagickFormatInfo EveluateFileFormat(HttpContext context, string sourceType)
    {
        var acceptHeader = context.Request.Headers[HeaderNames.Accept];
        var result = StringWithQualityHeaderValue.ParseList(acceptHeader);
        if (result.Any(e => e.Value == "webp"))
        {
            return MagickNET.SupportedFormats.First(e => e.Format == MagickFormat.WebP);
        }

        return GetMagickFormatFromMimeType(sourceType);
    }

    static IMagickFormatInfo GetMagickFormatFromMimeType(string mimeType)
    {
        return mimeType.ToLower() switch
        {
            "image/jpeg" => MagickNET.SupportedFormats.First(e => e.Format == MagickFormat.Jpeg),
            "image/png" => MagickNET.SupportedFormats.First(e => e.Format == MagickFormat.Png),
            "image/gif" => MagickNET.SupportedFormats.First(e => e.Format == MagickFormat.Gif),
            "image/tiff" => MagickNET.SupportedFormats.First(e => e.Format == MagickFormat.Tiff),
            "image/bmp" => MagickNET.SupportedFormats.First(e => e.Format == MagickFormat.Bmp),
            "image/webp" => MagickNET.SupportedFormats.First(e => e.Format == MagickFormat.WebP),
            _ => throw new ArgumentException($"Unknown MIME type: {mimeType}"),
        };
    }

    private string GetFileExtension(IMagickFormatInfo info)
    {
        return info.Format switch
        {
            MagickFormat.Jpg => "jpg",
            MagickFormat.Jpeg => "jpeg",
            MagickFormat.Png => "png",
            MagickFormat.Gif => "gif",
            MagickFormat.Tiff => "tiff",
            MagickFormat.Bmp => "bmp",
            MagickFormat.WebP => "webp",
            _ => "",
        };
    }
}
