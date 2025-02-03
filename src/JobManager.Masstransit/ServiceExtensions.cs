using Hackathon.Video.SharedKernel;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace JobManager.Masstransit;

public static class ServiceExtensions
{
    public static void ConfigureDispatcher(this IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<VideoProcessingConsumers>();
            x.AddConsumer<CustomerNotificationConsumers>();
            x.UsingAmazonSqs((context, cfg) =>
            {
                cfg.Host("us-east-1", h =>
                {
                    // h.Scope("dev", true);
                });
                cfg.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter( false));
            });
            
            x.ConfigureHealthCheckOptions(options =>
            {
                options.Name = "masstransit";
                options.MinimalFailureStatus = HealthStatus.Unhealthy;
                options.Tags.Add("health");
            });
        });
        
        services.AddSingleton<IDispatcher, Dispatcher>();
    }
}