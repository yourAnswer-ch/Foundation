using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Azure;

namespace Foundation.Services.ImageProcessor.Core.Caching;

public class CacheService(IAzureClientFactory<BlobServiceClient> factory)
{
    private const string ProfilesContainer = "profiles";
    private const string ClientId = "FlowcptStorageAccount";

    private BlobContainerClient? _container;

    private async Task<BlobContainerClient> GetBlobContainerClient()
    {
        if (_container != null)
            return _container;

        var client = factory.CreateClient(ClientId);
        _container = client.GetBlobContainerClient("cache");
        await _container.CreateIfNotExistsAsync();
        return _container;
    }

    public async Task<(Stream stream, string contentType)?> TryGetFileFromCache(string path)
    {
        var container = await GetBlobContainerClient();
        var blob = container.GetBlobClient(path);
        var exists = await blob.ExistsAsync();
        if (!exists)
            return default;

        BlobProperties properties = await blob.GetPropertiesAsync();

        if(properties.ContentLength == 0)
            return default;

        var stream = await blob.OpenReadAsync();

        return (stream, properties.ContentType);
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
