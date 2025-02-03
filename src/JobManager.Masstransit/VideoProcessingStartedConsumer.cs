using Hackathon.Video.SharedKernel;
using Hackathon.Video.SharedKernel.Events;
using JobManager.Domain.Dto;
using JobManager.Domain.ValueObjects;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace JobManager.Masstransit;

public class VideoProcessingConsumers(
    ILogger<VideoProcessingConsumers> logger,
    IUseCase<UpdateJobStatusDto> useCase)
    : IConsumer<VideoProcessingStarted>,
        IConsumer<VideoProcessingCompleted>,
        IConsumer<VideoProcessingFailed>
{
    public async Task Consume(ConsumeContext<VideoProcessingStarted> context)
    {
        var request =
            new UpdateJobStatusDto(context.Message.UserId, context.Message.JobId, JobStatus.InProgress);
        await useCase.ExecuteAsync(request);
    }

    public async Task Consume(ConsumeContext<VideoProcessingCompleted> context)
    {
        var request =
            new UpdateJobStatusDto(context.Message.UserId, context.Message.JobId, JobStatus.Completed);
        await useCase.ExecuteAsync(request);
    }

    public async Task Consume(ConsumeContext<VideoProcessingFailed> context)
    {
        var request =
            new UpdateJobStatusDto(context.Message.UserId, context.Message.JobId, JobStatus.Error);
        await useCase.ExecuteAsync(request);
    }
}