using Hackathon.Video.SharedKernel;
using JobManager.Controllers.Contracts;
using JobManager.Domain.Dto;

namespace JobManager.Controllers;

public class JobManagerService(
    IUseCase<CreateJobDto, JobDto> createJobUseCase,
    IUseCase<Guid, IReadOnlyList<JobDto>> getUserJobsUseCase,
    IUseCase<GetJobDetailDto, JobDto> getJobDetailUseCase)
    : IJobManagerService
{
    public Task<JobDto> CreateJobAsync(CreateJobDto job)
    {
        return createJobUseCase.ExecuteAsync(job);
    }

    public Task UpdateJobAsync(UpdateJobStatusDto job)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<JobDto>> ListJobsAsync(Guid userId)
    {
        return getUserJobsUseCase.ExecuteAsync(userId);
    }

    public Task<JobDto> GetOneAsync(Guid userId, Guid jobId)
    {
        return getJobDetailUseCase.ExecuteAsync(new GetJobDetailDto(userId, jobId));
    }
}