using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Foundation.Services.ImageProcessor.Core.Filters.Images;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Diagnostics;

namespace Foundation.Services.ImageProcessor.Core;

public class ImageProcessorMiddleware(
    RequestDelegate next,
    ImageFilter imageFilter,
    ILogger<ImageProcessorMiddleware> log,
    IAzureClientFactory<BlobServiceClient> factory)
{
    private const string ProfilesContainer = "profiles";
    private const string ClientId = "FlowcptStorageAccount";
    // https://saflowcptdev.blob.core.windows.net/profiles/3K6ehNLhnRsPfWhzjugmea/6XpV5QaK3nsDxNmSsVeTPA/1JutRAEbEZdsSwrMYJZqN8
    // https://saflowcptdev.blob.core.windows.net/profiles/3K6ehNLhnRsPfWhzjugmea/6O76ONdIxUOlO9U2Sf7GqI/0McKfF64pp25ZVpmq3kQT4

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/files", out var remainingPath))
            return;

        var stopwatch = Stopwatch.StartNew();
        try
        {
            var client = GetBlobClient(remainingPath);

            var exist = await client.ExistsAsync();
            if (!exist)
            {
                await ReturnNotfound(context);
                return;
            }

            BlobProperties properties = await client.GetPropertiesAsync();
            var stream = await client.OpenReadAsync();

            if (properties.ContentType.StartsWith("image/"))
            {
                await imageFilter.Filter(context, stream, properties.ContentType);
            }
            else
            {
                context.Response.ContentType = properties.ContentType;
                await stream.CopyToAsync(context.Response.Body);
            }
        }
        finally
        {
            LogRequest(context, stopwatch);
        }
    }

    private void LogRequest(HttpContext context, Stopwatch stopwatch)
    {
        stopwatch.Stop();

        var logLevel = context.Response.StatusCode >= 400 ? LogLevel.Warning : LogLevel.Information;
        log.Log(logLevel, "Request file - Path: {0} - Status: {1} - Duration {2:N0}ms", context.Request.Path, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
    }

    private async Task ReturnNotfound(HttpContext context)
    {
        var problemDetails = new ProblemDetails
        {
            Status = 404,
            Title = "Resource Not Found",
            Detail = "The resource you are looking for does not exist.",
            Instance = context.Request.Path
        };

        // Serialize the ProblemDetails object to JSON
        var json = JsonSerializer.Serialize(problemDetails);

        // Set the response content type and status code
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = 404;

        // Write the JSON response
        await context.Response.WriteAsync(json);
    }

    private BlobClient GetBlobClient(PathString path)
    {
        var client = factory.CreateClient(ClientId);
        var container = client.GetBlobContainerClient($"{ProfilesContainer}");
        return container.GetBlobClient(path);
    }
}