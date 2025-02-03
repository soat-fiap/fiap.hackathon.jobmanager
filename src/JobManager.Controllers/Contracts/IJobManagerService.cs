using JobManager.Domain.Dto;

namespace JobManager.Controllers.Contracts;

public interface IJobManagerService
{
    Task<JobDto> CreateJobAsync(CreateJobDto job);
    
    Task UpdateJobAsync(UpdateJobStatusDto job);
    
    Task<IReadOnlyList<JobDto>> ListJobsAsync(string userId);

    Task<JobDto> GetOneAsync(string userId, Guid jobId);
}