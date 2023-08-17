using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using CommandLine;

namespace CloudLogger.Commands;

public enum EnvironmentEnum
{
    dev,
    prd
}

[Verb("env", HelpText = "set environment --> env <dev/prd>")]
internal class EnvironmentVerb : IAction
{
    [Value(0, Required = false)]
    public EnvironmentEnum EnvironmentEnum { get; set; } = EnvironmentEnum.dev;

    public void Action()
    {
        Program.host?.Stop();

        Console.WriteLine($"Getting connection string to {EnvironmentEnum.ToString().ToUpper()} from KeyVault...");

        try
        {
            var credential = new DefaultAzureCredential(true);

            var client = new SecretClient(vaultUri: new Uri(_environments[EnvironmentEnum]), credential: credential);
            var secret = client.GetSecret("Logging--EventHub--Client--ConnectionString");

            var connection = new EventHubConnection(secret.Value.Value);

            Console.Write($"Try to connect to host: {connection.Host}...");

            Program.host?.Connect(connection, false).Wait(); //ToDo

            Console.WriteLine($"Connected.");

            Program.SetHeadline();

        }
        catch (Exception e)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"An error occured while trying to connect to environment;");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(e.Message);
            Console.ForegroundColor = ConsoleColor.White;

            Program.SetHeadline();
        }
    }

    private static readonly IDictionary<EnvironmentEnum, string> _environments = new Dictionary<EnvironmentEnum, string>
        {
            {EnvironmentEnum.dev, "https://kv-flowcpt-dev.vault.azure.net/"},
            {EnvironmentEnum.prd, "https://kv-flowcpt-prd.vault.azure.net/"},
        };
}
