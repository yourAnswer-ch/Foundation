using System.Diagnostics;
using System.Text;
using Foundation.Hosting.Info.Models;
using Microsoft.Extensions.Logging;

namespace Foundation.Hosting.Info;

public static class LoggerExtension
{
    public static void LogStartup(this ILogger log, string name)
    {
        try
        {
            var status = SystemStatus.GetInstance();

            var writer = new StringBuilder();
            writer.AppendLine();
            writer.AppendLine();
            writer.AppendLine($"       %%%%%%    ");
            writer.AppendLine($"      %%%%%%                ### {name} ###");
            writer.AppendLine($"     %%%%%%      ");
            writer.AppendLine($"    %%%%%%       version:   {status.Entry.FileVersion}");
            writer.AppendLine($"   %%%%%%%%%%%%  hostname:  {status.HostName}");
            writer.AppendLine($"  %%%%%%%%%%%%   system     {status.HostOS}");
            writer.AppendLine($"         %%%%    process:   {status.ProcessID}");
            writer.AppendLine($"        %%%      ");
            writer.AppendLine($"       %%%       ");
            writer.AppendLine($"      %%         entry:     {status.Entry.Name}");
            writer.AppendLine($"     %           directory: {status.Directory}");
            writer.AppendLine();

            log.LogInformation(writer.ToString());
        }
        catch(Exception ex)
        {
            //DoNothing
            Trace.WriteLine(ex.Message);
        }
    }

    public static void LogStop(this ILogger log, string name)
    {
        log.LogWarning($" ==> Service: {name} is shutting down.");
    }
}
