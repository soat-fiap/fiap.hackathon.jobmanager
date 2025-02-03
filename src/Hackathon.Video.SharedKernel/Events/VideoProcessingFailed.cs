namespace Hackathon.Video.SharedKernel.Events;

public record VideoProcessingFailed(string UserId, string JobId, string Message);