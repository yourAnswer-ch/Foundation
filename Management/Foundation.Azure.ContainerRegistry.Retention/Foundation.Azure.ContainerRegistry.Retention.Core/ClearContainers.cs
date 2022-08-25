using Azure.Containers.ContainerRegistry;
using Foundation.Processing.Pipeline;
using Microsoft.Extensions.Logging;

namespace Foundation.Azure.ContainerRegistry.Retention.Core;

public class ClearContainers : Command
{
    private readonly ILogger _log;
    private readonly ContainerRegistryClient _client;

    public ClearContainers(ContainerRegistryClient client, ILogger<ClearContainers> log)
    {
        _log = log;
        _client = client;
    }

    public async Task<Result> ExecuteAsync(CommandContext context)
    {
        var expiration = DateTime.UtcNow.Subtract(TimeSpan.FromDays(180));

        _log.LogInformation($"ClearContainers - Start cleaning up container registry - expiration: {expiration} - min count: 20 - endpoint: {_client.Endpoint}");

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
                var tags = string.Join('|', manifest.Tags);
                context.Messages.Add($"Image: {manifest.RepositoryName} - tags: {tags} - last update: {manifest.LastUpdatedOn.ToString("dd/MM/yyyy HH:mm")}");
                _log.LogInformation($"## Delete image: {manifest.RepositoryName} - last update: {manifest.LastUpdatedOn} - tags: {tags} - digest: {manifest.Digest}");
                foreach (var tagName in manifest.Tags)
                {
                    await image.DeleteTagAsync(tagName);
                }

                await image.DeleteAsync();
            }
        }

        return Result.Next();
    }
}

public class CommandContext
{
    public IList<string> Messages { get; }

    public CommandContext()
    {
        Messages = new List<string>();
    }
}