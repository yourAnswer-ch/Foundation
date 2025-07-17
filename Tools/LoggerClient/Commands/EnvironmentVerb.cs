using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using CommandLine;

namespace CloudLogger.Commands;

[Verb("env", HelpText = "set environment --> env <dev/prd>")]
internal class EnvironmentVerb : IAction
{
    [Value(0, Required = false)]
    public string Environment { get; set; } = "hbdev";

    public void Action()
    {
        Program.host?.Stop();
        
        var config = GetEnvironment(Environment);
        Console.WriteLine($"Connection: {config.Host}");

        try
        {
            var credential = new InteractiveBrowserCredential();

            //var connection = new EventHubConnection(secret.Value.Value);

            Console.Write($"Try to connect to host: {config.Host}...");

            Program.host?.Connect(config.Host, config.Queue, false, credential).Wait(); //ToDo

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

    private (string Host, string Queue) GetEnvironment(string environment)
    {
        return environment switch
        {
            "dev" => ("eh-ff-dev-log.servicebus.windows.net", "ff-dev-log"),
            "prd" => ("eh-ff-prd-log.servicebus.windows.net", "ff-prd-log"),
            "hbdev" => ("hellobusinessdev.servicebus.windows.net", "hb-dev-logger"),
            _ => throw new ArgumentException($"Unknown environment: {environment}")
        };
    }
}



// public enum EnvironmentEnum
// {
//     dev,
//     prd,
//     hbdev
// }
//
// [Verb("env", HelpText = "set environment --> env <dev/prd>")]
// internal class EnvironmentVerb : IAction
// {
//     [Value(0, Required = false)]
//     public EnvironmentEnum EnvironmentEnum { get; set; } = EnvironmentEnum.dev;
//
//     public void Action()
//     {
//         Program.host?.Stop();
//
//         Console.WriteLine($"Getting connection string to {EnvironmentEnum.ToString().ToUpper()} from KeyVault...");
//
//         try
//         {
//             var credential = new DefaultAzureCredential(
//                 new DefaultAzureCredentialOptions
//                 {
//                     TenantId = "719063d3-d297-4fb1-b874-9ddc0df73352"
//                 });
//
//             //var credential = new InteractiveBrowserCredential();
//
//             var client = new SecretClient(vaultUri: new Uri(_environments[EnvironmentEnum]), credential: credential);
//             var secret = client.GetSecret("Logging--EventHub--Client--ConnectionString");
//
//             var connection = new EventHubConnection(secret.Value.Value);
//
//             Console.Write($"Try to connect to host: {connection.Host}...");
//
//             Program.host?.Connect(connection, false).Wait(); //ToDo
//
//             Console.WriteLine($"Connected.");
//
//             Program.SetHeadline();
//
//         }
//         catch (Exception e)
//         {
//             Console.WriteLine();
//             Console.ForegroundColor = ConsoleColor.Red;
//             Console.WriteLine($"An error occured while trying to connect to environment;");
//             Console.ForegroundColor = ConsoleColor.Yellow;
//             Console.WriteLine(e.Message);
//             Console.ForegroundColor = ConsoleColor.White;
//
//             Program.SetHeadline();
//         }
//     }
//
//     private static readonly IDictionary<EnvironmentEnum, string> _environments = new Dictionary<EnvironmentEnum, string>
//     {
//         {EnvironmentEnum.dev, "https://kv-flowcpt-dev.vault.azure.net/"},
//         {EnvironmentEnum.prd, "https://kv-flowcpt-prd.vault.azure.net/"},
//         {EnvironmentEnum.hbdev, "https://kv-flowcpt-prd.vault.azure.net/"},
//     };
// }
