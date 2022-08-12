using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Foundation.Hosting.Info.Configuration;
using Foundation.Hosting.Info.Models;

namespace Foundation.Hosting.Info;

[ApiController]
[Route("[controller]")]
public class StatusController : ControllerBase
{
    private readonly InfoConfig Config;

    public StatusController(IConfiguration config)
    {
        Config = config.GetInfoConfig();
    }

    [HttpGet]
    [Route("{key}")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult GetStatus([FromRoute] string key)
    {
        if (!Config.EnableStatusController || string.IsNullOrWhiteSpace(key) || Config.Key != key)
            return NotFound();

        var status = SystemStatus.GetInstance(Config.ShowAssemblies, Config.ShowEnvironmentVariables);

        //if (!Config.ShowAssemblies)
        //{
        //    status.Entry = null;
        //    status.References = null;
        //}

        //if (!Config.ShowEnvironmentVariables)
        //{
        //    status.EnvironmentVariables = null;
        //}

        return new ObjectResult(status);
    }
}
