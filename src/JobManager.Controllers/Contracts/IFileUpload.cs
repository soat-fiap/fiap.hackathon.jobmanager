using JobManager.Domain.Dto;

namespace JobManager.Controllers.Contracts;

public interface IFileUpload
{
    Task<string> CreateUploadUrl(JobDto job);
    
    Task<string> CreateDownloadUrl(JobDto job);
}