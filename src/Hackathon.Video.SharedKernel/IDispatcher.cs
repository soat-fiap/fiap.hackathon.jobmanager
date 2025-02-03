namespace Hackathon.Video.SharedKernel;

public interface IDispatcher
{
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default(CancellationToken));
}