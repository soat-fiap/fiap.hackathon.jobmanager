using JobManager.Domain.ValueObjects;

namespace JobManager.Domain.Dto;

public record UpdateJobStatusDto(string UserId, Guid JobId, JobStatus Status);
