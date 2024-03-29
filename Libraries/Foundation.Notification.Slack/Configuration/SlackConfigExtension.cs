﻿using Microsoft.Extensions.Configuration;

namespace Foundation.Notification.Slack.Configuration;

public static class SlackConfigExtension
{
    public static SlackConfig GetSlackConfig(this IConfiguration config, string key = "Notification:Slack")
    {
        return config.GetSection(key).Get<SlackConfig>();
    }
}
