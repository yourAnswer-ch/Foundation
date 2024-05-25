using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Foundation.Services.ImageProcessor.Core.Filters.Images;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Azure;

namespace Foundation.Services.ImageProcessor.Core;

public class ImageProcessorMiddleware(RequestDelegate next, ImageFilter imageFilter, IAzureClientFactory<BlobServiceClient> factory)
{
    private const string ProfilesContainer = "profiles";
    private const string ClientId = "FlowcptStorageAccount";
    // https://saflowcptdev.blob.core.windows.net/profiles/3K6ehNLhnRsPfWhzjugmea/6XpV5QaK3nsDxNmSsVeTPA/1JutRAEbEZdsSwrMYJZqN8
    // https://saflowcptdev.blob.core.windows.net/profiles/3K6ehNLhnRsPfWhzjugmea/6O76ONdIxUOlO9U2Sf7GqI/0McKfF64pp25ZVpmq3kQT4

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            if (!context.Request.Path.StartsWithSegments("/files", out var remainingPath))
                return;

            var client = GetBlobClient(remainingPath);

            BlobProperties properties = await client.GetPropertiesAsync();
            var stream = await client.OpenReadAsync();

            if (properties.ContentType.StartsWith("image/"))
            {
                await imageFilter.Filter(context, stream);
            }
            else
            {
                context.Response.ContentType = properties.ContentType;
                await stream.CopyToAsync(context.Response.Body);
            }

        }
        finally
        {
            await next(context);
        }
    }

    private BlobClient GetBlobClient(PathString path)
    {
        var client = factory.CreateClient(ClientId);
        var container = client.GetBlobContainerClient($"{ProfilesContainer}");
        return container.GetBlobClient(path);
    }
}