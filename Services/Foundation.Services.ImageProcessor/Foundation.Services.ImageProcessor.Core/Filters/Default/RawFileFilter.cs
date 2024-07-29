using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;

namespace Foundation.Services.ImageProcessor.Core.Filters.Default
{
    public class RawFileFilter : IFilter
    {
        public async Task Filter(HttpContext context, BlobClient client, BlobProperties properties)
        {
            var stream = await client.OpenReadAsync();
            context.Response.ContentType = properties.ContentType;
            await stream.CopyToAsync(context.Response.Body);
        }
    }
}
