using Foundation.Azure.CertManager.Core.Configuration;
using Foundation.Processing.Pipeline;
using Microsoft.Extensions.Logging;

namespace Foundation.Azure.CertManager.Core.Steps;

public class LetsEncryptValidate : Command
{
    private readonly ILogger _log;

    public LetsEncryptValidate(ILogger<LetsEncryptValidate> log)
    {
        _log = log;
    }

    public async Task<Result> ExecuteAsync(Context context, CertificateConfig domain)
    {
        var delay = TimeSpan.FromSeconds(60);
        
        _log.LogInformation($"{domain.DomainName} - Validate lets encript order.");

        bool valid;
        var alreadyValidated = new List<string>();

        do
        {
            valid = true;
            
            _log.LogInformation($"{domain.DomainName} - Wait for {delay.TotalSeconds} seconds...");
            await Task.Delay(delay);

            foreach (var challenge in context.Challenges)
            {
                if (alreadyValidated.Contains(challenge.Token))
                    continue;

                var v = await challenge.Validate();
                _log.LogInformation($"{domain.DomainName} - Challenge type: {v.Type} status: {v.Status}");

                switch(v.Status)
                {
                    case Certes.Acme.Resource.ChallengeStatus.Valid:
                        alreadyValidated.Add(challenge.Token);
                        break;

                    case Certes.Acme.Resource.ChallengeStatus.Invalid:
                        _log.LogError($"{domain.DomainName} - Domain validation failed - Error: {v.Error}");
                        throw new SystemException($"{domain.DomainName} - Domain validation failed - Error: {v.Error}");

                    default:
                        valid = false; 
                        break;
                }
            }
        } while (!valid);

        return Result.Next();
    }
}
