using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;

namespace Foundation.Services.ImageProcessor.Core.Filters;

public interface IFilter
{
    Task Filter(BlobClient client, HttpContext context, string contentType);
}
