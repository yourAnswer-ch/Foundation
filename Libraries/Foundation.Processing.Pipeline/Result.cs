namespace Foundation.Processing.Pipeline;

public enum FlowControl
{
    Next,
    Skiped,
    Exit,
    Failed
}

public class Result
{
    public FlowControl FlowControl { get; private set; }

    public object? Value { get; private set; }

    internal Result(FlowControl flowControl, object? result)
    {
        FlowControl = flowControl;
        Value = result;
    }

    public static Result Next()
    {
        return new Result(FlowControl.Next, null);
    }

    public static Result Next(object? result)
    {
        return new Result(FlowControl.Next, result);
    }

    public static Result Skiped()
    {
        return new Result(FlowControl.Skiped, null);
    }

    public static Result Skiped(object? result)
    {
        return new Result(FlowControl.Skiped, result);
    }

    public static Result Exit()
    {
        return new Result(FlowControl.Exit, null);
    }

    public static Result Exit(object? result)
    {
        return new Result(FlowControl.Exit, result);
    }

    public static Result Failed()
    {
        return new Result(FlowControl.Failed, null);
    }

    public static Result Failed(object? result)
    {
        return new Result(FlowControl.Failed, result);
    }
}
