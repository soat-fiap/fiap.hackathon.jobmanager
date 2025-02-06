using Hackathon.Video.SharedKernel;
using Hackathon.Video.SharedKernel.Events;
using JetBrains.Annotations;
using JobManager.Domain.Dto;
using JobManager.Masstransit;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;

namespace JobManager.Tests.Masstransit;

[TestSubject(typeof(CustomerNotificationConsumers))]
public class CustomerNotificationConsumersTest
{
    [Fact]
    public async Task Consume_VideoProcessingCompleted_SendsNotification()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<CustomerNotificationConsumers>>();
        var useCaseMock = new Mock<IUseCase<NotificationMessageDto>>();
        var consumer = new CustomerNotificationConsumers(loggerMock.Object, useCaseMock.Object);
        var contextMock = new Mock<ConsumeContext<VideoProcessingCompleted>>();
        var message = new VideoProcessingCompleted(Guid.NewGuid(), Guid.NewGuid());
        contextMock.SetupGet(c => c.Message).Returns(message);

        // Act
        await consumer.Consume(contextMock.Object);

        // Assert
        useCaseMock.Verify(useCase => useCase.ExecuteAsync(It.Is<NotificationMessageDto>(dto =>
            dto.UserId == message.UserId && dto.Message == $"Job {message.JobId} completed")), Times.Once);
    }

    [Fact]
    public async Task Consume_VideoProcessingFailed_SendsNotification()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<CustomerNotificationConsumers>>();
        var useCaseMock = new Mock<IUseCase<NotificationMessageDto>>();
        var consumer = new CustomerNotificationConsumers(loggerMock.Object, useCaseMock.Object);
        var contextMock = new Mock<ConsumeContext<VideoProcessingFailed>>();
        var message = new VideoProcessingFailed(Guid.NewGuid(), Guid.NewGuid(), "Error");
        contextMock.SetupGet(c => c.Message).Returns(message);

        // Act
        await consumer.Consume(contextMock.Object);

        // Assert
        useCaseMock.Verify(useCase => useCase.ExecuteAsync(It.Is<NotificationMessageDto>(dto =>
                dto.UserId == message.UserId &&
                dto.Message == $"Job {message.JobId} Failed. Reason: {message.Message}")),
            Times.Once);
    }
}