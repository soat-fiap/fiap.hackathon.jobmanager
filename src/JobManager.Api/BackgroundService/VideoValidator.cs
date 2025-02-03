using System.Text.Json.Serialization;
using Amazon.SQS;
using Amazon.SQS.Model;
using Hackathon.Video.SharedKernel;
using Hackathon.Video.SharedKernel.Events;
using JobManager.Controllers.Contracts;
using MassTransit.JobService;
using Microsoft.Extensions.Options;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace JobManager.Api.BackgroundService;

public class VideoValidator(
    ILogger<VideoValidator> logger,
    IOptions<VideoReceivedSettings> settings,
    IAmazonSQS sqs,
    IServiceScopeFactory serviceScopeFactory,
    IDispatcher dispatcher)
    : IHostedService, IDisposable
{
    private Timer? _timer;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(async state => await PollMessagesAsync(), null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
        return Task.CompletedTask;
    }

    private async Task PollMessagesAsync()
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        var scopedJobManagerService =
            scope.ServiceProvider.GetRequiredService<IJobManagerService>();
        
        logger.LogInformation("Polling messages from SQS");
        var request = new ReceiveMessageRequest
        {
            QueueUrl = settings.Value.QueueUrl,
            MaxNumberOfMessages = 2,
            WaitTimeSeconds = 5
        };

        var response = await sqs.ReceiveMessageAsync(request);
        logger.LogInformation("Received {Count} messages", response.Messages.Count);

        foreach (var message in response.Messages)
        {
            var notificationMessage = JsonSerializer.Deserialize<VideoFileNotificationMessage>(message.Body);
            // Process the message
            if (notificationMessage.Items?.Length > 0)
            {
                var x = notificationMessage.Items.First();
                var bucket = x.VideoDetails.Bucket.Name;
                var userId = Guid.Parse(x.VideoDetails.File.Key.Split("/")[0]);
                var jobId = Guid.Parse(x.VideoDetails.File.Key.Split("/")[1]);

                var job = await scopedJobManagerService.GetOneAsync(userId, jobId);
                await dispatcher.PublishAsync(new VideoReceived(bucket, userId, jobId, job.Snapshots));
                await sqs.DeleteMessageAsync(settings.Value.QueueUrl, message.ReceiptHandle);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        logger.LogInformation("Stopping the service");

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}