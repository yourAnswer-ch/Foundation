namespace CloudLogger;

internal class Columns
{
    public bool ShowTimestamp { get; set; } = true;

    public bool ShowLogLevel { get; set; } = true;

    public bool ShowHost { get; set; } = true;

    public bool ShowApp { get; set; } = true;

    public bool ShowName { get; set; } = false;

    public bool ShowCorrelationId { get; set; } = false;

    public bool ShowMessage { get; set; } = true;
}