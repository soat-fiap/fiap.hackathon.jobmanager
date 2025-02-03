using JobManager.Domain.Entities;

namespace JobManager.Domain.Contracts;

public interface IJobRepository
{
    Task SaveAsync(Job job);
    
    Task UpdateAsync(Job job);
    
    Task<IReadOnlyList<Job>> GetJobsAsync(string userId);

    Task<Job> GetJobAsync(string userId, Guid jobId);
}