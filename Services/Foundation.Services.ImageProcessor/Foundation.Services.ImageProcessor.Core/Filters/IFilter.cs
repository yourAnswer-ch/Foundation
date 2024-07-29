using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;

namespace Foundation.Services.ImageProcessor.Core.Filters;

public interface IFilter
{
    Task Filter(HttpContext context, BlobClient client, BlobProperties properties);
}
