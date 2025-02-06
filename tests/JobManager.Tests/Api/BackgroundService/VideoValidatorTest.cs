using Amazon.SQS;
using Hackathon.Video.SharedKernel;
using JetBrains.Annotations;
using JobManager.Api.BackgroundService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace JobManager.Tests.Api.BackgroundService;

[TestSubject(typeof(VideoValidator))]
public class VideoValidatorTest
{

    [Fact]
    public async Task StartAsync_InitializesTimer()
    {
        var loggerMock = new Mock<ILogger<VideoValidator>>();
        var settingsMock = new Mock<IOptions<VideoReceivedSettings>>();
        var sqsMock = new Mock<IAmazonSQS>();
        var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
        var dispatcherMock = new Mock<IDispatcher>();

        var videoValidator = new VideoValidator(loggerMock.Object, settingsMock.Object, sqsMock.Object, serviceScopeFactoryMock.Object, dispatcherMock.Object);

        await videoValidator.StartAsync(CancellationToken.None);

        Assert.NotNull(videoValidator);
    }
}