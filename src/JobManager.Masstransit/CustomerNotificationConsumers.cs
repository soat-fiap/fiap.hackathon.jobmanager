using Hackathon.Video.SharedKernel;
using Hackathon.Video.SharedKernel.Events;
using JobManager.Domain.Dto;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace JobManager.Masstransit;

public class CustomerNotificationConsumers(
    ILogger<CustomerNotificationConsumers> logger,
    IUseCase<NotificationMessageDto> useCase)
    : IConsumer<VideoProcessingCompleted>,
        IConsumer<VideoProcessingFailed>
{
    public async Task Consume(ConsumeContext<VideoProcessingCompleted> context)
    {
        await Notify(new NotificationMessageDto(context.Message.UserId, $"Job {context.Message.JobId} completed"));
    }

    public async Task Consume(ConsumeContext<VideoProcessingFailed> context)
    {
        await Notify(new NotificationMessageDto(context.Message.UserId, $"Job {context.Message.JobId} Failed. Reason: {context.Message.Message}"));
    }
    
    private Task Notify(NotificationMessageDto notificationMessageDto)
    {
        return useCase.ExecuteAsync(notificationMessageDto);
    }
    
    
}