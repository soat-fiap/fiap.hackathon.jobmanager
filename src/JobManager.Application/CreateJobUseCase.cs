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
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Snapshots = request.SnapshotsCount
        };

        await jobManagerRepository.SaveAsync(job);
        return new JobDto(job.UserId, job.Id, job.Status, job.Snapshots, 1, string.Empty);
    }
}