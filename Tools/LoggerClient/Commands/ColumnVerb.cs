using CommandLine;

namespace CloudLogger.Commands;

internal enum SwitchEnum
{
    on,
    off
}

[Verb("column", HelpText = "Activates/deactivates individual columns, column (Timestamp|LogLevel|Host|App|Name|CorrelationId|Message)  <on/off>, ")]
internal class ColumnVerb : IAction
{
    [Value(0, Required = true)]
    public required string Name { get; set; }

    [Value(1, Required = true)]
    public SwitchEnum SwitchEnum { get; set; }

    public void Action()
    {
        switch(Name.ToLower())
        {
            case "timestamp":
                Program.columns.ShowTimestamp = getSwitch(SwitchEnum);
                break;

            case "loglevel":
                Program.columns.ShowLogLevel = getSwitch(SwitchEnum);
                break;

            case "host":
                Program.columns.ShowHost = getSwitch(SwitchEnum);
                break;

            case "app":
                Program.columns.ShowApp = getSwitch(SwitchEnum);
                break;

            case "name":
                Program.columns.ShowName = getSwitch(SwitchEnum);
                break;

            case "correlationid":
                Program.columns.ShowCorrelationId = getSwitch(SwitchEnum);
                break;

            case "Message":
                Program.columns.ShowMessage = getSwitch(SwitchEnum);
                break;
        }

        bool getSwitch(SwitchEnum value)
        {
            return value switch
            {
                SwitchEnum.on => true,
                SwitchEnum.off => false,
                _ => false
            };
        }  

        Console.WriteLine($"Column: {Name} is now turned {SwitchEnum}.");
    }
}
