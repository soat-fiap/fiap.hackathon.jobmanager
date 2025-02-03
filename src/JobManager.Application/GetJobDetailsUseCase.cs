using Hackathon.Video.SharedKernel;
using JobManager.Domain.Contracts;
using JobManager.Domain.Dto;

namespace JobManager.Application;

public class GetJobDetailsUseCase(IJobRepository jobRepository) : IUseCase<GetJobDetailDto, JobDto>
{
    public async Task<JobDto> ExecuteAsync(GetJobDetailDto request)
    {
        var job = await jobRepository.GetJobAsync(request.UserId, request.JobId);
        return new JobDto(job.UserId, job.Id, job.Status, job.Snapshots, job.SnapshotsProcessed, job.VideoPath);
    }
}