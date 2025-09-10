using Azure.Storage.Blobs;
using Foundation.Services.ImageProcessor.Core.Caching;
using Foundation.Services.ImageProcessor.Core.Configuration;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Foundation.Services.ImageProcessor.Core;

public static class ImageProcessorExtension
{
    public static void AddImageProcessor(this IServiceCollection services)
    {
        services.AddOptions<FileHandlerOptions>()
            .BindConfiguration("FileHandler")
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddAzureClients(builder =>
        {
            builder.AddClient<BlobServiceClient, BlobClientOptions>((_, credential, provider) =>
            {
                var config = provider.GetRequiredService<IOptions<FileHandlerOptions>>();
                return new BlobServiceClient(config.Value.StorageUrl, credential);
            }).WithName("ImageProcessor");

        });
        
        services.AddTransient<CacheService>();
    }
}
