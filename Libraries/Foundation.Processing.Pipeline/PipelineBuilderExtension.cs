using Foundation.Processing.Pipeline.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Foundation.Processing.Pipeline;

public static class PipelineBuilderExtension
{
    public static void AddPipeline(this IServiceCollection services, Action<IPipelineBuilder> builder)
    {
        var b = new PipelineBuilder(services);
        builder.Invoke(b);
        
        services.AddTransient<IPipeline>(e => new Pipeline(e, b.Definitions));
    }
}