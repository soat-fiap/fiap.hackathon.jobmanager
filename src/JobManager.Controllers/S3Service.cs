using Amazon.S3;
using Amazon.S3.Model;
using JobManager.Controllers.Contracts;
using JobManager.Domain.Dto;

namespace JobManager.Controllers;

public class S3Service(IAmazonS3 s3Client) : IFileUpload
{
    private const string Bucket = "hackathon-video-processor";

    public Task<string> CreateUploadUrl(JobDto job)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = Bucket,
            Key = $"{job.UserId}/{job.JobId}/video.mkv",
            Expires = DateTime.UtcNow.Add(TimeSpan.FromMinutes(10)),
            Verb = HttpVerb.PUT,
            Protocol = Protocol.HTTPS,
            ContentType = "video/x-matroska"
        };
        
        // Add file size limitation
        // request.Headers["x-amz-meta-max-file-size"] = maxFileSize.ToString();
        return s3Client.GetPreSignedURLAsync(request);
    }
    
    public Task<string> CreateDownloadUrl(JobDto job)
    {
        return s3Client.GetPreSignedURLAsync(new GetPreSignedUrlRequest
        {
            BucketName = Bucket,
            Key = $"{job.UserId}/{job.JobId}/images.zip",
            Expires = DateTime.UtcNow.Add(TimeSpan.FromMinutes(10)),
            Verb = HttpVerb.GET,
        });
    }
}