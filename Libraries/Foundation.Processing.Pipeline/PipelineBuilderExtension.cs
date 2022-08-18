using Foundation.Processing.Pipeline.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Foundation.Processing.Pipeline;

public static class PipelineBuilderExtension
{
    public static void AddPipeline(this IServiceCollection services, Action<IPipelineBuilder> builder)
    {
        var b = new PipelineBuilder(services);
        builder.Invoke(b);
        
        services.AddTransient<IPipeline>(e => new Pipeline(b.Definitions, e.GetRequiredService<ILogger<Pipeline>>()));
    }

}