using Hackathon.Video.SharedKernel;
using JobManager.Domain.Contracts;
using JobManager.Domain.Dto;
using JobManager.Domain.Entities;

namespace JobManager.Application;

public class CreateJobUseCase(IJobRepository jobManagerRepository) : IUseCase<CreateJobDto, JobDto>
{
    public async Task<JobDto> ExecuteAsync(CreateJobDto request)
    {
        var job = new Job()
        {
            Id = Guid.NewGuid().ToString(),
            UserId = request.UserId.ToString(),
            Snapshots = request.SnapshotsCount
        };

        await jobManagerRepository.SaveAsync(job);
        return new JobDto(Guid.Parse(job.UserId), Guid.Parse(job.Id), job.Status, job.Snapshots, 1, string.Empty);
    }
}