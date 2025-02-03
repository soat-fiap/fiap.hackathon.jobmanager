using JobManager.Domain.ValueObjects;

namespace JobManager.Domain.Dto;

public record JobDto(string UserId, string JobId, JobStatus Status, int Snapshots, int SnapshotsProcessed, string VideoPath);