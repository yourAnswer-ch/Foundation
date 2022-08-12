using System.Diagnostics;
using System.Reflection;

namespace Foundation.Hosting.Info.Models;

public class AssemblyStatus
{
    public string Name { get; private set; }

    public string FileVersion { get; private set; }

    public string ProductVersion { get; private set; }

    public string Version => ProductVersion == "-" ? FileVersion : ProductVersion;

    public AssemblyStatus() {
        Name = "Unknown";
        FileVersion = "-";
        ProductVersion = "-";
    }

    public AssemblyStatus(Assembly assembly)
    {
        var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);

        Name = assembly.GetName()?.Name ?? "Unknown";
        FileVersion = fvi.FileVersion ?? "-";
        ProductVersion = fvi.ProductVersion ?? "-";
    }

    public AssemblyStatus(AssemblyName assembly)
    {
        Name = assembly.Name ?? "Unknown";
        FileVersion = assembly.Version?.ToString() ?? "-";
        ProductVersion = "-";
    }

    public override string ToString()
    {
        return $"Assembly: {Name} - FileVersion: {FileVersion} - ProductVersion: {ProductVersion}";
    }
}
