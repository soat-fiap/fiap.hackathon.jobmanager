using JetBrains.Annotations;
using JobManager.Application;
using JobManager.Domain.Contracts;
using JobManager.Domain.Dto;
using JobManager.Domain.Entities;
using JobManager.Domain.ValueObjects;
using Moq;

namespace JobManager.Tests.Application.UseCases;

[TestSubject(typeof(UpdateJobStatusUseCase))]
public class UpdateJobStatusUseCaseTest
{
    [Fact]
    public async Task ExecuteAsync_UpdatesJobStatus_WhenJobExists()
    {
        var jobRepositoryMock = new Mock<IJobRepository>();
        var job = new Job
            { UserId = "b15b5195-ced8-4e88-aab8-906fb23dcc6a", Id = "b15b5195-ced8-4e88-aab8-906fb23dcc6a" };
        job.SetStatus(JobStatus.InProgress);
        jobRepositoryMock.Setup(repo => repo.GetJobAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(job);
        var useCase = new UpdateJobStatusUseCase(jobRepositoryMock.Object);
        var request = new UpdateJobStatusDto(Guid.NewGuid(), Guid.NewGuid(), JobStatus.Completed);

        await useCase.ExecuteAsync(request);

        jobRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<Job>(j => j.Status == JobStatus.Completed)),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ThrowsException_WhenJobDoesNotExist()
    {
        var jobRepositoryMock = new Mock<IJobRepository>();
        jobRepositoryMock.Setup(repo => repo.GetJobAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync((Job?)null);
        var useCase = new UpdateJobStatusUseCase(jobRepositoryMock.Object);
        var request = new UpdateJobStatusDto(Guid.NewGuid(), Guid.NewGuid(), JobStatus.Completed);

        await Assert.ThrowsAsync<Exception>(() => useCase.ExecuteAsync(request));
    }

    [Fact]
    public async Task ExecuteAsync_ThrowsException_WhenRepositoryFails()
    {
        var jobRepositoryMock = new Mock<IJobRepository>();
        jobRepositoryMock.Setup(repo => repo.GetJobAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ThrowsAsync(new Exception("Repository failure"));
        var useCase = new UpdateJobStatusUseCase(jobRepositoryMock.Object);
        var request = new UpdateJobStatusDto(Guid.NewGuid(), Guid.NewGuid(), JobStatus.Completed);

        await Assert.ThrowsAsync<Exception>(() => useCase.ExecuteAsync(request));
    }
}