using Microsoft.AspNetCore.Builder;

namespace Foundation.Services.ImageProcessor.Core;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseImageProcessor(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ImageProcessorMiddleware>();
    }
}