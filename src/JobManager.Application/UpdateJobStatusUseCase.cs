using Hackathon.Video.SharedKernel;
using JobManager.Domain.Contracts;
using JobManager.Domain.Dto;

namespace JobManager.Application;

public class UpdateJobStatusUseCase(IJobRepository jobRepository) : IUseCase<UpdateJobStatusDto>
{
    public async Task ExecuteAsync(UpdateJobStatusDto request)
    {
        var job = await jobRepository.GetJobAsync(request.UserId, request.JobId);
        job.SetStatus(request.Status);

        await jobRepository.UpdateAsync(job);
    }
}