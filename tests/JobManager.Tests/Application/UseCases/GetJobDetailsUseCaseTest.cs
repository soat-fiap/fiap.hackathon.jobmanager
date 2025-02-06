using JetBrains.Annotations;
using JobManager.Application;
using JobManager.Domain.Contracts;
using JobManager.Domain.Dto;
using JobManager.Domain.Entities;
using JobManager.Domain.ValueObjects;
using Moq;

namespace JobManager.Tests.Application.UseCases;

[TestSubject(typeof(GetJobDetailsUseCase))]
public class GetJobDetailsUseCaseTest
{

   [Fact]
    public async Task ExecuteAsync_ReturnsJobDetails_WhenJobExists()
    {
        var jobRepositoryMock = new Mock<IJobRepository>();
        var job = new Job { UserId = "b15b5195-ced8-4e88-aab8-906fb23dcc6a", Id = "b15b5195-ced8-4e88-aab8-906fb23dcc6a", Snapshots = 5, SnapshotsProcessed = 3, VideoPath = "path/to/video" };
        job.SetStatus(JobStatus.Open);
        jobRepositoryMock.Setup(repo => repo.GetJobAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(job);
        var useCase = new GetJobDetailsUseCase(jobRepositoryMock.Object);
        var request = new GetJobDetailDto(Guid.NewGuid(), Guid.NewGuid());

        var result = await useCase.ExecuteAsync(request);

        Assert.NotNull(result);
        Assert.Equal(job.UserId, result.UserId.ToString());
        Assert.Equal(job.Id, result.JobId.ToString());
        Assert.Equal(job.Status, result.Status);
        Assert.Equal(job.Snapshots, result.Snapshots);
        Assert.Equal(job.SnapshotsProcessed, result.SnapshotsProcessed);
        Assert.Equal(job.VideoPath, result.VideoPath);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsNull_WhenJobDoesNotExist()
    {
        var jobRepositoryMock = new Mock<IJobRepository>();
        jobRepositoryMock.Setup(repo => repo.GetJobAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync((Job?)null);
        var useCase = new GetJobDetailsUseCase(jobRepositoryMock.Object);
        var request = new GetJobDetailDto(Guid.NewGuid(), Guid.NewGuid());

        var result = await useCase.ExecuteAsync(request);

        Assert.Null(result);
    }

    [Fact]
    public async Task ExecuteAsync_ThrowsException_WhenRepositoryFails()
    {
        var jobRepositoryMock = new Mock<IJobRepository>();
        jobRepositoryMock.Setup(repo => repo.GetJobAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ThrowsAsync(new Exception("Repository failure"));
        var useCase = new GetJobDetailsUseCase(jobRepositoryMock.Object);
        var request = new GetJobDetailDto(Guid.NewGuid(), Guid.NewGuid());

        await Assert.ThrowsAsync<Exception>(() => useCase.ExecuteAsync(request));
    }

}