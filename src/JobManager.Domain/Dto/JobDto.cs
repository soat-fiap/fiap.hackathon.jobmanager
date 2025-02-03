using JobManager.Domain.ValueObjects;

namespace JobManager.Domain.Dto;

public record JobDto(Guid UserId, Guid JobId, JobStatus Status, int Snapshots, int SnapshotsProcessed, string VideoPath);