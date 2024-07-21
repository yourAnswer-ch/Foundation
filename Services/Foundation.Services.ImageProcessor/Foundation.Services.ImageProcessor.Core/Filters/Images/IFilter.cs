using Microsoft.AspNetCore.Http;

namespace Foundation.Services.ImageProcessor.Core.Filters.Images;

public interface IFilter
{
    Task Filter(HttpContext context, Stream stream, string mimeType);
}
