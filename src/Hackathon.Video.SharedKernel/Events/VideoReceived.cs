namespace Hackathon.Video.SharedKernel.Events;

public record VideoReceived(string Bucket, Guid UserId, Guid JobId, int Frames);