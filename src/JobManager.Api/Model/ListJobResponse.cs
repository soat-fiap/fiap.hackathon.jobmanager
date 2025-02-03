using JobManager.Domain.ValueObjects;

namespace JobManager.Api.Model;

public record ListJobResponse(Guid JobId, JobStatus JobStatus, int Snapshots);