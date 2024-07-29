using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Services.ImageProcessor.Core.Filters.Default
{
    public class RawFileFilter : IFilter
    {
        public async Task Filter(BlobClient client, HttpContext context, string contentType)
        {
            var stream = await client.OpenReadAsync();
            context.Response.ContentType = contentType;
            await stream.CopyToAsync(context.Response.Body);
        }
    }
}
