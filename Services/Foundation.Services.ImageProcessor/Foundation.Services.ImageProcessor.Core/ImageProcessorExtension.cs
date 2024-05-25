using Foundation.Services.ImageProcessor.Core.Configuration;
using Foundation.Services.ImageProcessor.Core.Filters.Images;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Foundation.Services.ImageProcessor.Core;

public static class ImageProcessorExtension
{
    public static IServiceCollection AddImageProcessor(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAzureClients(builder =>
        {
            builder.AddBlobServiceClient(configuration.GetBlobStorageConnectionString()).WithName("FlowcptStorageAccount");
        });

        services.AddSingleton<ImageFilter>();

        return services;
    }
}
