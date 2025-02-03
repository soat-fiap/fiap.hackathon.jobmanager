using Hackathon.Video.SharedKernel;
using JobManager.Controllers.Contracts;
using JobManager.Domain.Contracts;
using JobManager.Domain.Dto;

namespace JobManager.Controllers;

public class JobManagerService : IJobManagerService
{
    private readonly IUseCase<CreateJobDto, JobDto> _createJobUseCase;
    private readonly IUseCase<string, IReadOnlyList<JobDto>> _getUserJobsUseCase;
    private readonly IUseCase<GetJobDetailDto, JobDto> _getJobDetailUseCase;

    public JobManagerService(IUseCase<CreateJobDto, JobDto> createJobUseCase,
        IUseCase<string, IReadOnlyList<JobDto>> getUserJobsUseCase,
        IUseCase<GetJobDetailDto, JobDto> getJobDetailUseCase)
    {
        _createJobUseCase = createJobUseCase;
        _getUserJobsUseCase = getUserJobsUseCase;
        _getJobDetailUseCase = getJobDetailUseCase;
    }

    public Task<JobDto> CreateJobAsync(CreateJobDto job)
    {
        return _createJobUseCase.ExecuteAsync(job);
    }

    public Task UpdateJobAsync(UpdateJobStatusDto job)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<JobDto>> ListJobsAsync(string userId)
    {
        return _getUserJobsUseCase.ExecuteAsync(userId);
    }

    public Task<JobDto> GetOneAsync(string userId, Guid jobId)
    {
        return _getJobDetailUseCase.ExecuteAsync(new GetJobDetailDto(userId, jobId.ToString()));
    }
}