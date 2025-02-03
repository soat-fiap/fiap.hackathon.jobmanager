namespace Hackathon.Video.SharedKernel.Events;

public record VideoProcessingStarted(Guid UserId, Guid JobId);