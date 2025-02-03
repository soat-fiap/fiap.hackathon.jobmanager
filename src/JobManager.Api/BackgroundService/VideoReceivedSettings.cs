using System.Diagnostics.CodeAnalysis;
using JobManager.Cognito;
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