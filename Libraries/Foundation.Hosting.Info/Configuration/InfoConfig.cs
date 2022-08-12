namespace Foundation.Hosting.Info.Configuration;

internal class InfoConfig
{
    public string? Key { get; set; }

    public string AppName { get; set; } = "app name not configured";

    public bool EnableStatusController { get; set; } = true;

    public bool EnableConsoleStartupSummery { get; set; } = true;

    public bool ShowEnvironmentVariables { get; set; } = true;

    public bool ShowAssemblies { get; set; } = true;
}
