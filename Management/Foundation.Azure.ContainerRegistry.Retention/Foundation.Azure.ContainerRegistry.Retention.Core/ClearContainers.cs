using Azure;
using Azure.Containers.ContainerRegistry;
using Foundation.Processing.Pipeline;
using Microsoft.Extensions.Logging;

namespace Foundation.Azure.ContainerRegistry.Retention.Core
{

    public class ClearContainers : Command
    {
        private readonly ILogger _log;
        private readonly ContainerRegistryClient _client;


        public ClearContainers(ContainerRegistryClient client, ILogger<ClearContainers> log)
        {
            _log = log;
            _client = client;
        }

        public async Task ExecuteAsync()
        {
            var repositories = _client.GetRepositoryNamesAsync();
            await foreach (string name in repositories)
            {
                _log.LogInformation($"Repository: {name}");

                ContainerRepository repository = _client.GetRepository(name);

                var imageManifests = repository.GetAllManifestPropertiesAsync(ArtifactManifestOrder.LastUpdatedOnDescending);

                // Delete images older than the first three.
                await foreach (var imageManifest in imageManifests)
                {
                    var t = imageManifest.LastUpdatedOn;

                    RegistryArtifact image = repository.GetArtifact(imageManifest.Digest);
                    Console.WriteLine($"Deleting image with digest {imageManifest.Digest}.");
                    Console.WriteLine($"   Deleting the following tags from the image: ");
                    foreach (var tagName in imageManifest.Tags)
                    {
                        Console.WriteLine($"        {imageManifest.RepositoryName}:{tagName}");
                        //await image.DeleteTagAsync(tagName);
                    }
                    //await image.DeleteAsync();
                }

            }
        }
    }


    public class TestClearContainers : Command
    {
        public async Task<object> ExecuteAsync(Uri url)
        {
            var client = new HttpClient();
            var result = await client.GetAsync(url);
            var content = await result.Content.ReadAsStringAsync();

            return new { Content = content };
        }
    }

    public class WriteConsole : Command
    {
        public Task ExecuteAsync(string content)
        {
            Console.WriteLine(content);
            return Task.CompletedTask;
        }
    }
}