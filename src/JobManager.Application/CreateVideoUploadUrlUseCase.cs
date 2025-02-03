using Hackathon.Video.SharedKernel;
using JobManager.Domain.Contracts;
using JobManager.Domain.Dto;
using JobManager.Domain.Entities;

namespace JobManager.Application;

public class CreateVideoUploadUrlUseCase(IJobRepository jobRepository) : IUseCase<CreateJobVideoUploadUrl, string>
{
    public async Task<string> ExecuteAsync(CreateJobVideoUploadUrl request)
    {
        var job = await jobRepository.GetJobAsync(request.UserId, request.JobId);
        return await CreateUploadUrl(job);
    }
    
    private async Task<string> CreateUploadUrl(Job job)
    {
        throw new NotImplementedException();
    }
}