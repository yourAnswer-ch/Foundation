using SlackBotMessages.Models;
using System.Reflection;

namespace Foundation.Notification.Slack;

public class SlackReport
{
    public string Username { get; }
    
    public string IconUrl { get; }
    
    public string ErrorMessageFallback { get; }
    
    public string ErrorMessagePretext { get; }
    
    public string ErrorText { get; }
    
    public string SuccessText { get; }

    public SlackReport(
        string username, 
        string iconUrl, 
        string successText, 
        string errorText, 
        string errorMessageFallback,
        string errorMessagePretext)
    {
        Username = username;
        IconUrl = iconUrl;
        SuccessText = successText;
        ErrorText = errorText;
        ErrorMessageFallback = errorMessageFallback;
        ErrorMessagePretext = errorMessagePretext;
    }

    public async Task SendMessage(ISlackBotService slackBot, bool successs, Func<IEnumerable<Attachment>> attachments, Func<IEnumerable<Exception>> exceptions )
    {
        if (successs)
        {
            var a = attachments?.Invoke() ?? Array.Empty<Attachment>();
            await SendSuccessMessage(slackBot, a);
        }
        else
        {
            var e = exceptions?.Invoke() ?? Array.Empty<Exception>();
            await SendErrorMessage(slackBot, e);
        }
    }

    protected async Task SendErrorMessage(ISlackBotService slackBot, IEnumerable<Exception> exceptions)
    {            
        var message = new Message(ErrorText)
        {
            IconUrl = IconUrl,
            Username = Username,
            Attachments = new List<Attachment>
            {
                new Attachment
                {
                    Color = "danger",
                    Text = exceptions.Select(e => Format(e)).Aggregate((a, b) => $"{a}\n{b}"),
                    Fallback = ErrorMessageFallback,
                    Pretext = ErrorMessagePretext,
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

    protected async Task SendSuccessMessage(ISlackBotService slackBot, IEnumerable<Attachment> attachments)
    {
        var message = new Message(SuccessText)
        {
            IconUrl = IconUrl,
            Username = Username,
            Attachments = attachments?.ToList()
        };

        await slackBot.SendMessageAsync(message);
    }
}
