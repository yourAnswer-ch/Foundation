using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Foundation.Hosting.Info.Models;

public class SystemStatus
{
    public string App { get; private set; }

    public string Framework { get; private set; }

    public string Directory { get; private set; }

    public string ProcessID { get; private set; }

    public string HostOS { get; private set; }

    public string HostName { get; private set; }

    public AssemblyStatus Entry { get; private set; }

    public AssemblyStatus[] References { get; private set; }

    public Dictionary<string, string> EnvironmentVariables { get; private set; }

    public static SystemStatus GetInstance()
    {
        var assembly = Assembly.GetEntryAssembly();
        var references = assembly?.GetReferencedAssemblies().Select(a => new AssemblyStatus(a));
        var variables = Environment.GetEnvironmentVariables().Cast<DictionaryEntry>();

        return new SystemStatus
        (            
            Environment.Version.ToString(),
            Environment.CurrentDirectory,
            Environment.ProcessId.ToString(),
            RuntimeInformation.OSDescription,
            Environment.MachineName,
            assembly,
            references?.OrderBy(e => e.Name),
            variables.Select(e => new KeyValuePair<string, string>($"{e.Key}", $"{e.Value}")).OrderBy(e => e.Key)
        );
    }

    private SystemStatus(
        string framework,
        string directory,
        string processId,
        string hostOS,
        string hostName,
        Assembly? assembly,
        IEnumerable<AssemblyStatus>? references,
        IEnumerable<KeyValuePair<string, string>> environmentVariables)
    {
        if (assembly != null)
        {
            var name = assembly.GetName().Name ?? "unknown";
            App = name.LastIndexOf('.') > 0 ? name[(name.LastIndexOf('.') + 1)..] : name;
            Entry = new AssemblyStatus(assembly);
        }
        else
        {
            App = "unknown";
            Entry = new AssemblyStatus();
        }
        
        Framework = framework;
        Directory = directory;
        ProcessID = processId;
        HostOS = hostOS;
        HostName = hostName;
        References = references?.ToArray() ?? Array.Empty<AssemblyStatus>();
        EnvironmentVariables = environmentVariables.ToDictionary(k => k.Key, v => v.Value);
    }
}
