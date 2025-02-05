using JobManager.Domain.Entities;

namespace JobManager.Domain.Contracts;

public interface IJobRepository
{
    Task SaveAsync(Job job);
    
    Task UpdateAsync(Job job);
    
    Task<IReadOnlyList<Job>> GetJobsAsync(Guid userId);

    Task<Job?> GetJobAsync(Guid userId, Guid jobId);
}