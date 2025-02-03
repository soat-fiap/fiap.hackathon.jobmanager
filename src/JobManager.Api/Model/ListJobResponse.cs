using JobManager.Domain.ValueObjects;

namespace JobManager.Api.Model;

public record ListJobResponse(string JobId, JobStatus JobStatus);