using Foundation.Azure.ContainerRegistry.Retention;
using Foundation.Azure.ContainerRegistry.Retention.Core;
using Foundation.Processing.Pipeline;
using Foundation.Processing.Pipeline.Abstractions;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Foundation.ServiceBuilder.AzureDefault;
using Foundation.Notification.Slack;
using SlackBotMessages.Models;
using SlackBotMessages;
using System.Reflection;

var report = new SlackReport(
   username: "Container Registy maintinace",
   iconUrl: "https://azure.microsoft.com/svghandler/container-registry/?width=300&height=300",
   successText: $"Container Registy following images got cleand up.",
   errorText: $"Container Registy - Errors occurred during processing",
   errorMessageFallback: "Exceptions occurd check logs for details",
   errorMessagePretext: $"Exceptions occurd check logs for details {Emoji.X}");

var stack = DefaultAzureStack.Create
    .AddConfiguration()
    .AddLogging()
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

if (success)
{
    await SendSuccessMessage(slackBot, context.Messages);
}
else
{
    await SendErrorMessage(slackBot, pipeline.Exceptions);
}

await SendSuccessMessage(slackBot, context.Messages);

static async Task SendErrorMessage(ISlackBotService slackBot, IEnumerable<Exception> exceptions)
{
    var text = $"Container Registy - Errors occurred during processing";
    var message = new Message(text)
    {
        Username = "Container Registy maintinace",
        IconUrl = "https://azure.microsoft.com/svghandler/container-registry/?width=300&height=300",
        Attachments = new List<Attachment>
        {
            new Attachment
            {
                Color = "danger",
                Text = exceptions.Select(e => Format(e)).Aggregate((a, b) => $"{a}\n{b}"),
                Fallback = "Exceptions occurd check logs for details",
                Pretext = $"Exceptions occurd check logs for details {Emoji.X}",
            },
        }
    };

    await slackBot.SendMessageAsync(message);

    string Format(Exception ex)
    {
        if (ex == null)
            return "unknown exception";

        if (ex is TargetInvocationException m && m.InnerException != null)
            return $"{m.GetType().Name}: {m.Message}";

        return $"{ex.GetType().Name}: {ex.Message}";
    }

}

static async Task SendSuccessMessage(ISlackBotService slackBot, IEnumerable<string> messages)
{
    var attachments = messages.Select(e =>
    {
        return new Attachment
        {
            Text = e,
            Color = "good",
            Fallback = "Deletet images:",
            Pretext = $"Deletet images:",
        };
    });

    var text = $"Container Registy following images got cleand up.";
    var message = new Message(text)
    {
        Username = "Container Registy maintinace",
        IconUrl = "https://azure.microsoft.com/svghandler/container-registry/?width=300&height=300",
        Attachments = attachments.ToList()
    };

    await slackBot.SendMessageAsync(message);
}
