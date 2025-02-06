using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Hackathon.Video.SharedKernel;
using Hackathon.Video.SharedKernel.Events;
using JetBrains.Annotations;
using JobManager.Api.BackgroundService;
using JobManager.Controllers.Contracts;
using JobManager.Domain.Dto;
using JobManager.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Record = JobManager.Api.BackgroundService.Record;

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

        var videoValidator = new VideoValidator(loggerMock.Object, settingsMock.Object, sqsMock.Object,
            serviceScopeFactoryMock.Object, dispatcherMock.Object);

        await videoValidator.StartAsync(CancellationToken.None);

        Assert.NotNull(videoValidator);
    }

    [Fact]
    public async Task PollMessagesAsync_ProcessesMessagesSuccessfully()
    {
        var loggerMock = new Mock<ILogger<VideoValidator>>();
        var settingsMock = new Mock<IOptions<VideoReceivedSettings>>();
        var sqsMock = new Mock<IAmazonSQS>();
        var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
        var dispatcherMock = new Mock<IDispatcher>();
        var serviceScopeMock = new Mock<IServiceScope>();
        var jobManagerServiceMock = new Mock<IJobManagerService>();

        settingsMock.Setup(s => s.Value).Returns(new VideoReceivedSettings { QueueUrl = "testQueueUrl" });
        serviceScopeFactoryMock.Setup(f => f.CreateScope()).Returns(serviceScopeMock.Object);
        serviceScopeMock.Setup(s => s.ServiceProvider.GetService(typeof(IJobManagerService)))
            .Returns(jobManagerServiceMock.Object);
        sqsMock.Setup(s => s.ReceiveMessageAsync(It.IsAny<ReceiveMessageRequest>(), default))
            .ReturnsAsync(new ReceiveMessageResponse
            {
                Messages = new List<Message>
                {
                    new Message
                    {
                        Body = JsonSerializer.Serialize(new VideoFileNotificationMessage
                        {
                            Items = new[]
                            {
                                new Record()
                                {
                                    VideoDetails = new S3()
                                    {
                                        Bucket = new Bucket { Name = "testBucket" },
                                        File = new VideoObject { Key = "userId/jobId" }
                                    }
                                }
                            }
                        })
                    }
                }
            });
        jobManagerServiceMock.Setup(s => s.GetOneAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(new JobDto(Guid.NewGuid(), Guid.NewGuid(), JobStatus.Open, 10, 0, ""));

        var videoValidator = new VideoValidator(loggerMock.Object, settingsMock.Object, sqsMock.Object,
            serviceScopeFactoryMock.Object, dispatcherMock.Object);

        await videoValidator.StartAsync(default);
    }

    [Fact]
    public async Task PollMessagesAsync_LogsInformation_WhenNoMessagesReceived()
    {
        var loggerMock = new Mock<ILogger<VideoValidator>>();
        var settingsMock = new Mock<IOptions<VideoReceivedSettings>>();
        var sqsMock = new Mock<IAmazonSQS>();
        var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
        var dispatcherMock = new Mock<IDispatcher>();
        var serviceScopeMock = new Mock<IServiceScope>();

        settingsMock.Setup(s => s.Value).Returns(new VideoReceivedSettings { QueueUrl = "testQueueUrl" });
        serviceScopeFactoryMock.Setup(f => f.CreateScope()).Returns(serviceScopeMock.Object);
        sqsMock.Setup(s => s.ReceiveMessageAsync(It.IsAny<ReceiveMessageRequest>(), default))
            .ReturnsAsync(new ReceiveMessageResponse { Messages = new List<Message>() }).Verifiable();

        var videoValidator = new VideoValidator(loggerMock.Object, settingsMock.Object, sqsMock.Object,
            serviceScopeFactoryMock.Object, dispatcherMock.Object);

        await videoValidator.StartAsync(default);
    }

    [Fact]
    public async Task StopAsync_StopsTimer()
    {
        var loggerMock = new Mock<ILogger<VideoValidator>>();
        var settingsMock = new Mock<IOptions<VideoReceivedSettings>>();
        var sqsMock = new Mock<IAmazonSQS>();
        var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
        var dispatcherMock = new Mock<IDispatcher>();

        var videoValidator = new VideoValidator(loggerMock.Object, settingsMock.Object, sqsMock.Object,
            serviceScopeFactoryMock.Object, dispatcherMock.Object);

        await videoValidator.StartAsync(CancellationToken.None);
        await videoValidator.StopAsync(CancellationToken.None);
    }
}