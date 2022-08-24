using Foundation.Processing.Pipeline;
using Foundation.Processing.Pipeline.Abstractions;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Foundation.ServiceBuilder.AzureDefault;
using Foundation.Azure.CertManager.Core.Steps;
using Foundation.Azure.CertManager.Core;
using Microsoft.Extensions.Configuration;
using Foundation.Azure.CertManager.Core.Configuration;
using Microsoft.Extensions.Logging;
using SlackBotMessages.Models;
using SlackBotMessages;
using Certes;
using Foundation.Notification.Slack;

var stack = DefaultAzureStack.Create
    .AddConfiguration()
    .AddLogging()
    .AddServices(s =>
    {
        s.AddAzureClients(e =>
        {
            e.AddCertificateClient(new Uri("https://kv-fd-certificates.vault.azure.net/"));
        });

        s.AddPipeline(builder =>
        {            
            builder.AddCommand<AzureCheckIfIsExpired>();
            builder.AddCommand<LetsEncryptCreateAccount>();
            builder.AddCommand<LetsEncryptCreateOrder>();
            builder.AddCommand<LetsEncryptAuthorizeDns>();
            builder.AddCommand<AzureCreateTxtRecord, AzureRemoveTxtRecord>();
            builder.AddCommand<LetsEncryptValidate>();
            builder.AddCommand<LetsEncryptDownloadCert>();
            builder.AddCommand<AzureStoreCert>();
            builder.AddCommand<AzureRemoveTxtRecord>();
            //builder.AddCommand<AzureFrontDoor>();
            builder.AddExceptionFormater<AcmeRequestException>(Formator.Exception);
        });

        s.AddSlackBot();
    });

var provider = stack.Build();

var slack = provider.GetRequiredService<ISlackBotService>();
var pipeline = provider.GetRequiredService<IPipeline>();


var log = provider.GetRequiredService<ILoggerFactory>().CreateLogger("CertManager");
var config = provider.GetRequiredService<IConfiguration>().GetCertManagerConfig();

foreach (var domain in config.Certificates)
{
    log.LogInformation($"Process domain: {domain.DomainName} resource group: {domain.ResourceGroup}");

    var context = new Context();

    var success = await pipeline.ExecuteAsync(new { 
        Context = context, 
        Domain = domain 
    });

    if (context.IsValid)
        continue;

    if(success)
    {
        await SendSuccessMessage(slack, domain);
    }
    else
    {
        await SendErrorMessage(slack, pipeline.Exceptions, domain);
    }
}

static async Task SendErrorMessage(ISlackBotService slackBot, IEnumerable<Exception> exceptions, CertificateConfig domain)
{
    var text = $"Certificate renewal faild - Domain: {domain.DomainName}";
    var message = new Message(text)
    {
        Username = "Certificate maintinace",
        IconUrl = "https://miro.medium.com/max/700/1*8mpWApzQD5gZZlnYYUkbcA.png",
        Attachments = new List<Attachment>
                {
                    new Attachment
                    {
                        Fallback = "Exceptions occurd check logs for details",
                        Pretext = $"Exceptions occurd check logs for details {Emoji.X}",
                        Text = exceptions.Select(e => $"{e.GetType().Name}: {e.Message}").Aggregate((a, b) => $"{a}\n{b}"),
                        Color = "danger",
                    },
                }
    };

    await slackBot.SendMessageAsync(message);
}

static async Task SendSuccessMessage(ISlackBotService slackBot, CertificateConfig domain)
{
    var text = $"Certificate successfully renewed - Domain: {domain.DomainName}";
    var message = new Message(text)
    {
        Username = "Certificate maintinace",
        IconUrl = "https://miro.medium.com/max/700/1*8mpWApzQD5gZZlnYYUkbcA.png",
        Attachments = new List<Attachment>
                {
                    new Attachment
                    {
                        Fallback = "Verify the following domain",
                        Pretext = $"Verify the following domain {Emoji.HeavyCheckMark}",
                        Text = $"https://{domain.DomainName}",
                        Color = "good",
                    },
                }
    };

    await slackBot.SendMessageAsync(message);
}
