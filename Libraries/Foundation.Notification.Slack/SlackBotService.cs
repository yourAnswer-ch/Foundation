using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SlackBotMessages.Models;
using SlackBotMessages;
using Foundation.Notification.Slack.Configuration;

namespace Foundation.Notification.Slack;

public interface ISlackBotService
{
    Task<string?> SendMessageAsync(Message message);

    Task<string?> SendTestMessageAsync();
}

public class SlackBotService : ISlackBotService
{
    protected readonly SlackConfig _config;
    protected readonly ILogger<SlackBotService> _log;

    public SlackBotService(IConfiguration config, ILogger<SlackBotService> log)
    {
        _config = config.GetSlackConfig();
        _log = log;
    }

    public async Task<string?> SendMessageAsync(Message message)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_config?.WebHookUrl))
                throw new ArgumentException("Slack - WebHookUrl not configured.");

            var client = new SbmClient(_config.WebHookUrl);
            var value = await client.SendAsync(message);

            _log.LogInformation($"Slack - Message sent: {value}");

            return value;
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Slack - Fail to post message.");
        }

        return null;
    }

    public async Task<string?> SendTestMessageAsync()
    {
        var message = new Message()
        {
            Username = "Jobs Service",
            //IconUrl = "https://miro.medium.com/max/700/1*8mpWApzQD5gZZlnYYUkbcA.png",
            IconUrl = "https://azure.microsoft.com/svghandler/key-vault/?width=300&height=300",
            Attachments = new List<Attachment>
            {
                new Attachment
                {
                    Fallback = "This is a test message",
                    Pretext = $"This is a test message {Emoji.HeavyCheckMark}",
                    Text = $"This is a test message",
                    Color = "good",
                },
            }
        };

        return await SendMessageAsync(message);
    }
}
