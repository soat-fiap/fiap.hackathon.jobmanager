using JetBrains.Annotations;
using JobManager.Application;
using JobManager.Domain.Contracts;
using JobManager.Domain.Entities;
using JobManager.Domain.ValueObjects;
using Moq;

namespace JobManager.Tests.Application.UseCases;

[TestSubject(typeof(GetUserJobsUseCase))]
public class GetUserJobsUseCaseTest
{
    [Fact]
    public async Task ExecuteAsync_ReturnsJobs_WhenJobsExist()
    {
        var jobRepositoryMock = new Mock<IJobRepository>();
        var jobs = new List<Job>
        {
            new Job
            {
                UserId = "b15b5195-ced8-4e88-aab8-906fb23dcc6a", Id = "b15b5195-ced8-4e88-aab8-906fb23dcc6a",
                Snapshots = 5, SnapshotsProcessed = 3, VideoPath = "path/to/video"
            },
            new Job
            {
                UserId = "b15b5195-ced8-4e88-aab8-906fb23dcc6a", Id = "c25b5195-ced8-4e88-aab8-906fb23dcc6b",
                Snapshots = 10, SnapshotsProcessed = 10, VideoPath = "path/to/another/video"
            }
        };
        jobs[0].SetStatus(JobStatus.Open);
        jobs[1].SetStatus(JobStatus.Completed);
        jobRepositoryMock.Setup(repo => repo.GetJobsAsync(It.IsAny<Guid>())).ReturnsAsync(jobs);
        var useCase = new GetUserJobsUseCase(jobRepositoryMock.Object);

        var result = await useCase.ExecuteAsync(Guid.NewGuid());

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(jobs[0].Id, result[0].JobId.ToString());
        Assert.Equal(jobs[1].Id, result[1].JobId.ToString());
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsEmptyList_WhenNoJobsExist()
    {
        var jobRepositoryMock = new Mock<IJobRepository>();
        jobRepositoryMock.Setup(repo => repo.GetJobsAsync(It.IsAny<Guid>())).ReturnsAsync(new List<Job>());
        var useCase = new GetUserJobsUseCase(jobRepositoryMock.Object);

        var result = await useCase.ExecuteAsync(Guid.NewGuid());

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ExecuteAsync_ThrowsException_WhenRepositoryFails()
    {
        var jobRepositoryMock = new Mock<IJobRepository>();
        jobRepositoryMock.Setup(repo => repo.GetJobsAsync(It.IsAny<Guid>()))
            .ThrowsAsync(new Exception("Repository failure"));
        var useCase = new GetUserJobsUseCase(jobRepositoryMock.Object);

        await Assert.ThrowsAsync<Exception>(() => useCase.ExecuteAsync(Guid.NewGuid()));
    }
}