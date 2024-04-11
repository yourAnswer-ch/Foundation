using Foundation.Azure.ContainerRegistry.Retention.Core;
using Foundation.Processing.Pipeline;
using Foundation.Processing.Pipeline.Abstractions;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Foundation.ServiceBuilder.AzureDefault;
using Foundation.ServiceBuilder;
using Foundation.Notification.Slack;
using SlackBotMessages.Models;
using SlackBotMessages;

var report = new SlackReport(
   username: "Container Registy maintinace",
   iconUrl: "https://azure.microsoft.com/svghandler/container-registry/?width=300&height=300",
   successText: $"Container Registy following images got cleand up.",
   errorText: $"Container Registy - Errors occurred during processing",
   errorMessageFallback: "Exceptions occurd check logs for details",
   errorMessagePretext: $"Exceptions occurd check logs for details {Emoji.X}");

var stack = Stack.Create
    .AddDefaultConfiguration()
    .AddDefaultLogging()
    .AddServices(s =>
    {
        s.AddAzureClients(e =>
        {
            e.AddContianerRegistry();
        });

        s.AddPipeline(builder =>
        {
            builder.AddCommand<ClearContainers>();
        });

        s.AddSlackBot();
    });

var provider = stack.Build();

var pipeline = provider.GetRequiredService<IPipeline>();
var slackBot = provider.GetRequiredService<ISlackBotService>();

var context = new CommandContext();

var success = await pipeline.ExecuteAsync(new { Context = context });

if (success && context.Messages.Count == 0)
    return;

await report.SendMessage(slackBot, success, () =>
{
    return context.Messages
    .GroupBy(g => g.Key)
    .Select(g =>
    {
        return new Attachment
        {
            Color = "good",
            Text = g.Select(e => e.Value).Aggregate((a, b) => $"{a}\n{b}"),            
            Fallback = $"Repository: {g.Key}",
            Pretext = $"Repository: {g.Key}",
        };
    });
}, () => pipeline.Exceptions);
