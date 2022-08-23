using Microsoft.Extensions.Configuration;
using SlackBotMessages;
using SlackBotMessages.Models;
using Microsoft.Extensions.Logging;
using Foundation.Azure.CertManager.Core.Slack.Configuration;

namespace Foundation.Azure.CertManager.Core.Slack;

public interface ISlackBotService
{
    Task<string> SendMessageAsync(Message message);
    
    Task<string> SendTestMessageAsync();
}

public class SlackBotService : ISlackBotService
{
    protected readonly SlackConfig Config;
    protected readonly ILogger<SlackBotService> log;

    public SlackBotService(IConfiguration config, ILogger<SlackBotService> log)
    {
        Config = SlackConfig.BindConfig(config, "FlairFindr:Jobs:Slack");
        this.log = log;
    }       

    public async Task<string> SendMessageAsync(Message message)
    {
        try
        {
            var client = new SbmClient(Config.WebHookUrl);
            var value = await client.SendAsync(message);

            log.LogInformation($"Message sent to slack: {value}");

            return value;
        }
        catch(Exception ex)
        {
            log.LogError(ex, "Fail to post slack message.");
        }

        return null;
    }

    public async Task<string> SendTestMessageAsync()
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
            var client = new SbmClient(Config.WebHookUrl);
            var value = await client.SendAsync(message);

            log.LogInformation($"Message sent to slack: {value}");

            return value;
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Fail to post slack message.");
        }

        return null;
    }
}
