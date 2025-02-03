using JobManager.Domain.ValueObjects;

namespace JobManager.Domain.Dto;

public record UpdateJobStatusDto(Guid UserId, Guid JobId, JobStatus Status);
