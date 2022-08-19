using Azure.Containers.ContainerRegistry;
using Foundation.Processing.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Foundation.Azure.ContainerRegistry.Retention.Core;

public class ClearContainers : Command
{
    private readonly ILogger _log;
    private readonly ContainerRegistryClient _client;

    public ClearContainers(ContainerRegistryClient client, ILogger<ClearContainers> log, IConfiguration config)
    {
        _log = log;
        _client = client;
    }

    public async Task ExecuteAsync()
    {
        var expiration = DateTime.UtcNow.Subtract(TimeSpan.FromDays(180));

        var repositories = _client.GetRepositoryNamesAsync();
        await foreach (string name in repositories)
        {
            _log.LogInformation($"Repository: {name}");

            ContainerRepository repository = _client.GetRepository(name);

            var imageManifests = repository.GetAllManifestPropertiesAsync(ArtifactManifestOrder.LastUpdatedOnDescending);

            var count = 0;
            await foreach (var manifest in imageManifests)
            {
                var t = manifest.LastUpdatedOn;
                if(count++ < 20 || manifest.LastUpdatedOn > expiration)
                {
                    _log.LogInformation($"Skip image: {manifest.RepositoryName} - last update: {manifest.LastUpdatedOn} - created: {manifest.CreatedOn}");
                    continue;
                }

                RegistryArtifact image = repository.GetArtifact(manifest.Digest);
                _log.LogInformation($"## Delete image: {manifest.RepositoryName} - last update: {manifest.LastUpdatedOn} - tags: {string.Join('|', manifest.Tags)} - digest: {manifest.Digest}");
                foreach (var tagName in manifest.Tags)
                {
                    await image.DeleteTagAsync(tagName);
                }

                await image.DeleteAsync();
            }
        }
    }
}