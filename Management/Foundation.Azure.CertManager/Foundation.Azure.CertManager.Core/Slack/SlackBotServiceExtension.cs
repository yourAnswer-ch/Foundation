using Microsoft.Extensions.DependencyInjection;

namespace Foundation.Azure.CertManager.Core.Slack;

public static class SlackBotServiceExtension
{
    public static void AddSlackBot(this IServiceCollection services)
    {
        services.AddTransient<ISlackBotService, SlackBotService>();
    }
}
