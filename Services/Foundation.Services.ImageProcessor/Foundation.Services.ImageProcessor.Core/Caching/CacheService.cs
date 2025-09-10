using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Foundation.Services.ImageProcessor.Core.Configuration;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;

namespace Foundation.Services.ImageProcessor.Core.Caching;

public class CacheService(IAzureClientFactory<BlobServiceClient> factory, IConfiguration configuration)
{
    private BlobContainerClient? _container;
    
    private async Task<BlobContainerClient> GetBlobContainerClient()
    {
        if (_container != null)
            return _container;

        var client = factory.CreateClient("ImageProcessor");
        _container = client.GetBlobContainerClient("cache");
        await _container.CreateIfNotExistsAsync();
        return _container;
    }

    public async Task<bool> TryGetFileFromCache(string path, Func<Stream, string, Task> writer)
    {
        var container = await GetBlobContainerClient();
        var blob = container.GetBlobClient(path);
        var exists = await blob.ExistsAsync();
        if (!exists)
            return false;

        BlobProperties properties = await blob.GetPropertiesAsync();

        if (properties.ContentLength == 0)
            return false;

        using (var stream = await blob.OpenReadAsync())
        {
            await writer(stream, properties.ContentType);
            return true;
        }
    }

    public async Task SaveFileToCache(Stream stream, string path, string contentType)
    {
        var container = await GetBlobContainerClient();
        var blob = container.GetBlobClient(path);
        await blob.UploadAsync(stream, true);
        var headers = new BlobHttpHeaders
        {
            ContentType = contentType
        };

        await blob.SetHttpHeadersAsync(headers);
    }

    public async Task SaveFileToCache(string path, string contentType, Func<Stream, Task> writer)
    {
        var container = await GetBlobContainerClient();
        var blob = container.GetBlobClient(path);

        using (var stream = await blob.OpenWriteAsync(true))
        {
            await writer(stream);
        }

        var headers = new BlobHttpHeaders
        {
            ContentType = contentType
        };

        await blob.SetHttpHeadersAsync(headers);
    }
}
