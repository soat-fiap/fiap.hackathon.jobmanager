namespace Hackathon.Video.SharedKernel.Events;

public record VideoProcessingCompleted(Guid UserId, Guid JobId);