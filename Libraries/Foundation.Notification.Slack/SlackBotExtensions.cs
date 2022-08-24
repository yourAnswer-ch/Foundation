using Microsoft.Extensions.DependencyInjection;

namespace Foundation.Notification.Slack;

public static class SlackBotExtensions
{
    public static void AddSlackBot(this IServiceCollection services)
    {
        services.AddSingleton<ISlackBotService, SlackBotService>();
    }
}