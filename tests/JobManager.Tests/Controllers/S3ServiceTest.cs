using Amazon.S3;
using Amazon.S3.Model;
using JetBrains.Annotations;
using JobManager.Controllers;
using JobManager.Domain.Dto;
using JobManager.Domain.ValueObjects;
using Moq;

namespace JobManager.Tests.Controllers;

[TestSubject(typeof(S3Service))]
public class S3ServiceTest
{
    [Fact]
    public async Task CreateUploadUrl_ReturnsValidUrl()
    {
        // Arrange
        var s3ClientMock = new Mock<IAmazonS3>();
        var job = new JobDto(Guid.NewGuid(), Guid.NewGuid(), JobStatus.Open, 10, 10, "path");
        var expectedUrl = "https://example.com/upload";
        s3ClientMock.Setup(client => client.GetPreSignedURLAsync(It.IsAny<GetPreSignedUrlRequest>())).ReturnsAsync(expectedUrl);
        var service = new S3Service(s3ClientMock.Object);

        // Act
        var result = await service.CreateUploadUrl(job);

        // Assert
        Assert.Equal(expectedUrl, result);
    }

    [Fact]
    public async Task CreateUploadUrl_ThrowsException_WhenS3ClientFails()
    {
        // Arrange
        var s3ClientMock = new Mock<IAmazonS3>();
        var job = new JobDto(Guid.NewGuid(), Guid.NewGuid(), JobStatus.Open, 10, 10, "path");
        s3ClientMock.Setup(client => client.GetPreSignedURLAsync(It.IsAny<GetPreSignedUrlRequest>())).ThrowsAsync(new Exception("S3 failure"));
        var service = new S3Service(s3ClientMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => service.CreateUploadUrl(job));
    }

    [Fact]
    public async Task CreateDownloadUrl_ReturnsValidUrl()
    {
        // Arrange
        var s3ClientMock = new Mock<IAmazonS3>();
        var job = new JobDto(Guid.NewGuid(), Guid.NewGuid(), JobStatus.Open, 10, 10, "path");
        var expectedUrl = "https://example.com/download";
        s3ClientMock.Setup(client => client.GetPreSignedURLAsync(It.IsAny<GetPreSignedUrlRequest>())).ReturnsAsync(expectedUrl);
        var service = new S3Service(s3ClientMock.Object);

        // Act
        var result = await service.CreateDownloadUrl(job);

        // Assert
        Assert.Equal(expectedUrl, result);
    }

    [Fact]
    public async Task CreateDownloadUrl_ThrowsException_WhenS3ClientFails()
    {
        // Arrange
        var s3ClientMock = new Mock<IAmazonS3>();
        var job = new JobDto(Guid.NewGuid(), Guid.NewGuid(), JobStatus.Open, 10, 10, "path");
        s3ClientMock.Setup(client => client.GetPreSignedURLAsync(It.IsAny<GetPreSignedUrlRequest>())).ThrowsAsync(new Exception("S3 failure"));
        var service = new S3Service(s3ClientMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => service.CreateDownloadUrl(job));
    }
}