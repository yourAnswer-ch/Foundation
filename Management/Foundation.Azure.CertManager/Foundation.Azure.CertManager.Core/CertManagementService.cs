//using Certes.Acme;
//using Foundation.Azure.CertManager.Core.Configuration;
//using Foundation.Azure.CertManager.Core.Slack;
//using Foundation.Azure.CertManager.Core.Steps;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using SlackBotMessages;
//using SlackBotMessages.Models;

//namespace Foundation.Azure.CertManager.Core;

//public interface ICertManagementService
//{
//    Task Run();
//}

//public class CertManagementService : ICertManagementService
//{
//    private readonly ILogger _log;
//    private readonly ISlackBotService _slackBot;
//    private readonly CertManagerConfig _config;

//    public CertManagementService(IConfiguration config, ILogger<CertManagementService> log, ISlackBotService slackBot)
//    {
//        _log = log;
//        _slackBot = slackBot;
//        _config = new CertManagerConfig();
//        config.Bind(_config);
//    }

//    public async Task Run()
//    {
//        _log.LogInformation("Start updating certificates");
//        try
//        {
//            foreach (var domain in _config.CertManager.Domains)
//            {
//                _log.LogInformation($"Process domain: {domain.Name} resource group: {domain.ResourceGroup}");

//                var context = new Context(_config, _log);
//                await context.GetKeyVaultClient();

//                var isExpired = await context.IsExpired(domain.Name);
//                if (!isExpired)
//                {
//                    if(context.Errors)
//                        await SendMessage(context, domain.Name);
                    
//                    continue;
//                }

//                await context.CreateAccount(WellKnownServers.LetsEncryptV2);
//                await context.CreateOrder(domain.Name);
//                await context.AuthorizeDns();
//                await context.CreateAzureClient();
//                await context.CreateTxtRecord(domain.ResourceGroup, domain.Name);
//                await context.Validate();
//                await context.DownloadCert(domain.Name);
//                await context.StoreCert(domain.Name);
//                await context.RemoveTxtRecord(domain.ResourceGroup, domain.Name);
//                await context.UpdateCertificate(domain.ResourceGroup, _config.CertManager.FrontDoorName, domain.Name);

//                await SendMessage(context, domain.Name);
//            }
//        }
//        catch (Exception ex)
//        {
//            _log.LogError(ex, "Certificate update failed");
//        }
//        finally
//        {
//            _log.LogInformation("Done updating certificates");
//        }
//    }

//    private async Task SendMessage(Context context, string domain)
//    {
//        if (context.Errors)
//        {
//            var text = $"Certificate job faild. - Domain: {domain} - Utc Time: {DateTime.Now}";
//            var message = new Message(text)
//            {
//                Username = "Jobs Service",
//                IconUrl = "https://miro.medium.com/max/700/1*8mpWApzQD5gZZlnYYUkbcA.png",
//                Attachments = new List<Attachment>
//                {
//                    new Attachment
//                    {
//                        Fallback = "Multiple exceptions.",
//                        Pretext = $"Exceptions occurd check logstrem for details {Emoji.X}",
//                        Text = context.Exceptions.Select(e => $"{e.GetType().Name}: {e.Message}").Aggregate((a, b) => $"{a}\n{b}"),
//                        Color = "danger",
//                    },
//                }
//            };

//            await _slackBot.SendMessageAsync(message);
//        }
//        else
//        {
//            var text = $"Certificate has been regenerated. The delivery on FrontDoor was started. The exchange can take up to 60 minutes. - Domain: {domain}";
//            var message = new Message(text)
//            {
//                Username = "Jobs Service",
//                IconUrl = "https://miro.medium.com/max/700/1*8mpWApzQD5gZZlnYYUkbcA.png",
//                Attachments = new List<Attachment>
//                {
//                    new Attachment
//                    {
//                        Fallback = "Verify the following domains.",
//                        Pretext = $"Verify the following domains {Emoji.HeavyCheckMark}",
//                        Text = $"https://{domain}\n\nhttps://www.{domain}",
//                        Color = "good",
//                    },
//                }
//            };

//            await _slackBot.SendMessageAsync(message);
//        }
//    }
//}
