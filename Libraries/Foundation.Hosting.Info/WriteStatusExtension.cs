using Figgle;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Foundation.Hosting.Info.Models;
using Foundation.Hosting.Info.Configuration;

namespace Foundation.Hosting.Info;

public static class WriteStatusExtension
{
    public static IHost RegisterLifetimeLogger(this IHost host)
    {
        var output = new StringBuilder();
        try
        {
            var config = host.Services.GetRequiredService<IConfiguration>().GetInfoConfig(); 
            host.Services.RegisterLifetimeLogger();
            output.WriteStatus(config.AppName, config);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Status report exception: {ex.Message}");
        }
        finally
        {
            Console.WriteLine(output);
        }

        return host;
    }

    public static IHost WriteStatus(this IHost host, string appName)
    {
        var output = new StringBuilder();

        try
        {
            var config = host.Services.GetRequiredService<IConfiguration>().GetInfoConfig(); 
            output.WriteStatus(appName, config);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Status report exception: {ex.Message}");
        }
        finally
        {
            Console.WriteLine(output);
        }

        return host;
    }

    internal static void WriteStatus(this StringBuilder output, string appName, InfoConfig config)
    {
        if (!config.EnableConsoleStartupSummery)
            return;

        var status = SystemStatus.GetInstance();
        var version = status.Entry.Version;

        output.AppendLine(FiggleFonts.Standard.Render($"{appName} - {version}"));
        output.AppendLine();
        output.AppendLine($"Framework: {status.Framework}");
        output.AppendLine($"Directory: {status.Directory}");
        output.AppendLine($"ProcessID: {status.ProcessID}");
        output.AppendLine($"Host OS:   {status.HostOS}");
        output.AppendLine($"Host name: {status.HostName}");
        output.AppendLine();

        if (config.ShowAssemblies && status.References.Any())
        {
            output.AppendLine("Entry:");
            AppendAssemblyStatus(output, status.Entry);
            output.AppendLine();

            output.AppendLine("Assemblies:");
            foreach (var reference in status.References)
            {
                AppendAssemblyStatus(output, reference);
            }
            output.AppendLine();
        }

        if (config.ShowEnvironmentVariables && status.EnvironmentVariables.Any())
        {
            output.AppendLine("Environment variables:");
            foreach (var reference in status.EnvironmentVariables)
            {
                output.AppendLine($" - {reference.Key.PadRight(30)}: {reference.Value}");
            }
            output.AppendLine();
        }

        void AppendAssemblyStatus(StringBuilder builder, AssemblyStatus s)
        {
            builder.Append($"Name: {s.Name.PadRight(60)}");

            if (!string.IsNullOrEmpty(s.FileVersion))
                builder.Append($" - FileVersion: {s.FileVersion.PadRight(12)}");

            if (!string.IsNullOrEmpty(s.ProductVersion))
                builder.Append($" - ProductVersion: {s.ProductVersion}");

            builder.AppendLine();
        }
    }
}
