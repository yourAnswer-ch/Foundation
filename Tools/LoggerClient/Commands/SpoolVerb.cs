using CommandLine;

namespace CloudLogger.Commands;

public enum SpoolEnum
{
    on,
    off
}

[Verb("spool", HelpText = "output to file --> spool <on/off> <filename>")]
internal class SpoolVerb : IAction
{
    //normal options here
    [Value(0, Required = false)]
    public SpoolEnum SpoolEnum { get; set; }

    [Value(1, Required = false)]
    public required string SpoolToFile { get; set; }

    public void Action()
    {
        if(Program.writer == null) 
            throw new ArgumentNullException(nameof(Program));

        switch (SpoolEnum)
        {
            case SpoolEnum.off:
                Program.writer.Disabled = true;
                Console.WriteLine($"Stop writing Messages to '{Program.writer.FileName}'");
                break;

            case SpoolEnum.on:
                if (!string.IsNullOrEmpty(SpoolToFile))
                {
                    Program.writer.FileName = SpoolToFile;
                }
                Console.WriteLine($"Writing Messages to '{Program.writer.FileName}'");
                Program.writer.Disabled = false;
                break;
        }
    }
}
