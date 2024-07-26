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
        if(_container != null)
            return _container;

        var client = factory.CreateClient(ClientId);
        _container =  client.GetBlobContainerClient("cache");
        await _container.CreateIfNotExistsAsync();
        return _container;
    }

    public async Task<(Stream, string)?> TryGetFileFromCache(string path)
    {
        var container = await GetBlobContainerClient();
        var blob = container.GetBlobClient(path);
        var exists = await blob.ExistsAsync();
        if(!exists)
            return default;

        BlobProperties properties = await blob.GetPropertiesAsync();
        var stream = await blob.OpenReadAsync();

        return (stream, properties.ContentType);
    }

    public async Task SaveFileToCache(string path, Stream stream, string contentType)
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
}
