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
            builder.AddCommand<TestException>();
            //builder.AddCommand<AzureCheckIfIsExpired>();
            //builder.AddCommand<LetsEncryptCreateAccount>();
            //builder.AddCommand<LetsEncryptCreateOrder>();
            //builder.AddCommand<LetsEncryptAuthorizeDns>();
            //builder.AddCommand<AzureCreateTxtRecord, AzureRemoveTxtRecord>();
            //builder.AddCommand<LetsEncryptValidate>();
            //builder.AddCommand<LetsEncryptDownloadCert>();
            //builder.AddCommand<AzureStoreCert>();
            //builder.AddCommand<AzureRemoveTxtRecord>();
            //builder.AddCommand<AzureFrontDoor>();
            builder.AddExceptionFormater<AcmeRequestException>(Formator.Exception);
        });
    });

var provider = stack.Build();

var pipeline = provider.GetRequiredService<IPipeline>();


var log = provider.GetRequiredService<ILoggerFactory>().CreateLogger("CertManager");
var config = provider.GetRequiredService<IConfiguration>().GetCertManagerConfig();

foreach (var domain in config.Certificates)
{
    log.LogInformation($"Process domain: {domain.DomainName} resource group: {domain.ResourceGroup}");

    var context = new Context();

    //var isExpired = await context.IsExpired(domain.Name);
    //if (!isExpired)
    //{
    //if (context.Errors)
    //    await SendMessage(context, domain.Name);

    //  continue;
    //}

    await pipeline.ExecuteAsync(new { Context = context, Domain = domain });

    //await SendMessage(context, domain.Name);
}


//private static async Task SendMessage(Context context, string domain)
//{
//    if (context.Errors)
//    {
//        var text = $"Certificate job faild. - Domain: {domain} - Utc Time: {DateTime.Now}";
//        var message = new Message(text)
//        {
//            Username = "Jobs Service",
//            IconUrl = "https://miro.medium.com/max/700/1*8mpWApzQD5gZZlnYYUkbcA.png",
//            Attachments = new List<Attachment>
//                {
//                    new Attachment
//                    {
//                        Fallback = "Multiple exceptions.",
//                        Pretext = $"Exceptions occurd check logstrem for details {Emoji.X}",
//                        Text = context.Exceptions.Select(e => $"{e.GetType().Name}: {e.Message}").Aggregate((a, b) => $"{a}\n{b}"),
//                        Color = "danger",
//                    },
//                }
//        };

//        await _slackBot.SendMessageAsync(message);
//    }
//    else
//    {
//        var text = $"Certificate has been regenerated. The delivery on FrontDoor was started. The exchange can take up to 60 minutes. - Domain: {domain}";
//        var message = new Message(text)
//        {
//            Username = "Jobs Service",
//            IconUrl = "https://miro.medium.com/max/700/1*8mpWApzQD5gZZlnYYUkbcA.png",
//            Attachments = new List<Attachment>
//                {
//                    new Attachment
//                    {
//                        Fallback = "Verify the following domains.",
//                        Pretext = $"Verify the following domains {Emoji.HeavyCheckMark}",
//                        Text = $"https://{domain}\n\nhttps://www.{domain}",
//                        Color = "good",
//                    },
//                }
//        };

//        await _slackBot.SendMessageAsync(message);
//    }
//}

public class TestException : Command
{
    public Task<Result> ExecuteAsync()
    {
        return Task.FromResult(Result.Failed());
        //throw new AcmeRequestException();
    }

}