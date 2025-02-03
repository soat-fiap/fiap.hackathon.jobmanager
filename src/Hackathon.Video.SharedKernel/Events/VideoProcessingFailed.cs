namespace Hackathon.Video.SharedKernel.Events;

public record VideoProcessingFailed(Guid UserId, Guid JobId, string Message);