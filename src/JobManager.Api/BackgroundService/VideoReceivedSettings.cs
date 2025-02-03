using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;

namespace JobManager.Api.BackgroundService;

[ExcludeFromCodeCoverage]
public class VideoReceivedSettings
{
    public string QueueUrl { get; set; }
}

[ExcludeFromCodeCoverage]
public class VideoReceivedSettingsSetup(IConfiguration configuration) : IConfigureOptions<VideoReceivedSettings>
{
    public void Configure(VideoReceivedSettings options)
    {
        configuration
            .GetSection(nameof(VideoReceivedSettings))
            .Bind(options);
    }
}

[ExcludeFromCodeCoverage]
public static class ServiceExtensions
{
    public static void ConfigureVideoReceivedQueue(this IServiceCollection services)
    {
        services.ConfigureOptions<VideoReceivedSettingsSetup>();
    }
}