using Hackathon.Video.SharedKernel;
using Hackathon.Video.SharedKernel.Events;
using JetBrains.Annotations;
using JobManager.Domain.Dto;
using JobManager.Domain.ValueObjects;
using JobManager.Masstransit;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;

namespace JobManager.Tests.Masstransit;

[TestSubject(typeof(VideoProcessingConsumers))]
public class VideoProcessingConsumersTest
{
    [Fact]
    public async Task Consume_VideoProcessingStarted_UpdatesJobStatusToInProgress()
    {
        var loggerMock = new Mock<ILogger<VideoProcessingConsumers>>();
        var useCaseMock = new Mock<IUseCase<UpdateJobStatusDto>>();
        var consumer = new VideoProcessingConsumers(loggerMock.Object, useCaseMock.Object);
        var contextMock = new Mock<ConsumeContext<VideoProcessingStarted>>();
        var message = new VideoProcessingStarted(Guid.NewGuid(), Guid.NewGuid());
        contextMock.SetupGet(c => c.Message).Returns(message);

        await consumer.Consume(contextMock.Object);

        useCaseMock.Verify(useCase => useCase.ExecuteAsync(It.Is<UpdateJobStatusDto>(dto =>
                dto.UserId == message.UserId && dto.JobId == message.JobId && dto.Status == JobStatus.InProgress)),
            Times.Once);
    }

    [Fact]
    public async Task Consume_VideoProcessingCompleted_UpdatesJobStatusToCompleted()
    {
        var loggerMock = new Mock<ILogger<VideoProcessingConsumers>>();
        var useCaseMock = new Mock<IUseCase<UpdateJobStatusDto>>();
        var consumer = new VideoProcessingConsumers(loggerMock.Object, useCaseMock.Object);
        var contextMock = new Mock<ConsumeContext<VideoProcessingCompleted>>();
        var message = new VideoProcessingCompleted(Guid.NewGuid(), Guid.NewGuid());
        contextMock.SetupGet(c => c.Message).Returns(message);

        await consumer.Consume(contextMock.Object);

        useCaseMock.Verify(useCase => useCase.ExecuteAsync(It.Is<UpdateJobStatusDto>(dto =>
                dto.UserId == message.UserId && dto.JobId == message.JobId && dto.Status == JobStatus.Completed)),
            Times.Once);
    }

    [Fact]
    public async Task Consume_VideoProcessingFailed_UpdatesJobStatusToError()
    {
        var loggerMock = new Mock<ILogger<VideoProcessingConsumers>>();
        var useCaseMock = new Mock<IUseCase<UpdateJobStatusDto>>();
        var consumer = new VideoProcessingConsumers(loggerMock.Object, useCaseMock.Object);
        var contextMock = new Mock<ConsumeContext<VideoProcessingFailed>>();
        var message = new VideoProcessingFailed(Guid.NewGuid(), Guid.NewGuid(), "Error");
        contextMock.SetupGet(c => c.Message).Returns(message);

        await consumer.Consume(contextMock.Object);

        useCaseMock.Verify(useCase => useCase.ExecuteAsync(It.Is<UpdateJobStatusDto>(dto =>
            dto.UserId == message.UserId && dto.JobId == message.JobId && dto.Status == JobStatus.Error)), Times.Once);
    }
}