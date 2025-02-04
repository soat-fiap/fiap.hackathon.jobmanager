using Hackathon.Video.SharedKernel;
using JobManager.Domain.Contracts;
using JobManager.Domain.Dto;

namespace JobManager.Application;

public class GetUserJobsUseCase(IJobRepository jobRepository) : IUseCase<Guid, IReadOnlyList<JobDto>>
{
    public async Task<IReadOnlyList<JobDto>> ExecuteAsync(Guid request)
    {
        var jobs = await jobRepository.GetJobsAsync(request);
        return jobs.Select(job =>
            new JobDto(Guid.Parse(job.UserId),Guid.Parse(job.Id), job.Status, job.Snapshots, job.SnapshotsProcessed, job.VideoPath)).ToList();
    }
}