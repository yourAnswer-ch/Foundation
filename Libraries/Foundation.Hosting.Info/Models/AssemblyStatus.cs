using System.Diagnostics;
using System.Reflection;

namespace Foundation.Hosting.Info.Models;

public class AssemblyStatus
{
    public string Name { get; private set; }

    public string FileVersion { get; private set; }

    public string ProductVersion { get; private set; }

    public string Version { get; private set; }

    public AssemblyStatus() {
        Name = "Unknown";
        FileVersion = "-";
        ProductVersion = "-";
        Version = "-";
    }

    public AssemblyStatus(Assembly assembly)
    {
        var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);

        Name = assembly.GetName()?.Name ?? "Unknown";
        FileVersion = fvi.FileVersion ?? "-";
        ProductVersion = fvi.ProductVersion ?? "-";
        Version = !string.IsNullOrEmpty(fvi.FileVersion) ? fvi.FileVersion : $"{fvi.ProductMajorPart}.{fvi.ProductMinorPart}.{fvi.ProductBuildPart}";
    }

    public AssemblyStatus(AssemblyName assembly)
    {
        Name = assembly.Name ?? "Unknown";
        Version = assembly.Version?.ToString() ?? "-";
        FileVersion = assembly.Version?.ToString() ?? "-";
        ProductVersion = "-";        
    }

    public override string ToString()
    {
        return $"Assembly: {Name} - FileVersion: {FileVersion} - ProductVersion: {ProductVersion}";
    }
}
