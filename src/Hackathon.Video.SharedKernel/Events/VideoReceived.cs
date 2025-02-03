namespace Hackathon.Video.SharedKernel.Events;

public record VideoReceived(string Bucket, string UserId, string JobId, int Frames);