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
            var client = new SbmClient(_config.WebHookUrl);
            var value = await client.SendAsync(message);

            _log.LogInformation($"Message sent to slack: {value}");

            return value;
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Fail to post slack message.");
        }

        return null;
    }

    public async Task<string?> SendTestMessageAsync()
    {
        try
        {
            var message = new Message()
            {
                Username = "Jobs Service",
                IconUrl = "https://miro.medium.com/max/700/1*8mpWApzQD5gZZlnYYUkbcA.png",
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
            var client = new SbmClient(_config.WebHookUrl);
            var value = await client.SendAsync(message);

            _log.LogInformation($"Message sent to slack: {value}");

            return value;
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Fail to post slack message.");
        }

        return null;
    }
}