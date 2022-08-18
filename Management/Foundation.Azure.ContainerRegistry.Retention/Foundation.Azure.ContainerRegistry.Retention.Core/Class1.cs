using Foundation.Processing.Pipeline;

namespace Foundation.Azure.ContainerRegistry.Retention.Core
{
    public class ClearContainers : Command
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