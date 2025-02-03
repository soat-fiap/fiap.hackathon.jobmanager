namespace JobManager.Domain.Dto;

public record CreateJobDto(Guid UserId, int SnapshotsCount);