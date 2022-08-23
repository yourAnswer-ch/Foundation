using Foundation.Processing.Pipeline;
using Microsoft.Extensions.Logging;

namespace Foundation.Azure.CertManager.Core.Steps;

public class LetsEncryptValidate : Command
{
    private readonly ILogger<LetsEncryptValidate> _log;

    public LetsEncryptValidate(ILogger<LetsEncryptValidate> log)
    {
        _log = log;
    }

    public async Task<Result> ExecuteAsync(Context context)
    {
        foreach (var challenge in context.Challenges)
        {
            var v = await challenge.Validate();
            _log.LogInformation($"Challenge type: {v.Type} status: {v.Status}");
        }

        return Result.Next();
    }
}
