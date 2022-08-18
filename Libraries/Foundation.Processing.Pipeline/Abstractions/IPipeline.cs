using Microsoft.VisualBasic;

namespace Foundation.Processing.Pipeline.Abstractions
{
    public interface IPipeline
    {
        Task ExecuteAsync(IServiceProvider provieder, object parameters);
        //event Action<object> Finished;
    }
}