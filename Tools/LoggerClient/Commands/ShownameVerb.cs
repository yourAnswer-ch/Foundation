using CommandLine;

namespace CloudLogger.Commands;

internal enum SwitchEnum
{
    on,
    off
}

[Verb("showname", HelpText = "showname <on/off>, turn name print on or off")]
internal class ShownameVerb : IAction
{
    [Value(0, Required = true)]
    public SwitchEnum SwitchEnum { get; set; }

    public void Action()
    {
        switch (SwitchEnum)
        {
            case SwitchEnum.on:
                Program.showName = true;                    
                break;

            case SwitchEnum.off:
                Program.showName = false;
                break;
        };

        Console.WriteLine($"Show name is now {SwitchEnum}");
    }
}
