using Microsoft.Extensions.Configuration;

namespace Foundation.Azure.CertManager.Core.Slack.Configuration;

public class SlackConfig
{
    public string WebHookUrl { get; set; }

    public static SlackConfig BindConfig(IConfiguration config, string key = "Slack")
    {
        var instance = new SlackConfig();
        config.Bind(key, instance);
        return instance;
    }
}
