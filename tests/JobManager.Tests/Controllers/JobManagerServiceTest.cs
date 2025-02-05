using Hackathon.Video.SharedKernel;
using JetBrains.Annotations;
using JobManager.Controllers;
using JobManager.Domain.Dto;
using JobManager.Domain.ValueObjects;
using Moq;

namespace JobManager.Tests.Controllers;

[TestSubject(typeof(JobManagerService))]
public class JobManagerServiceTest
{
    [Fact]
    public async Task CreateJobAsync_ReturnsJobDto_WhenJobIsCreated()
    {
        // Arrange
        var createJobUseCaseMock = new Mock<IUseCase<CreateJobDto, JobDto>>();
        var userId = Guid.NewGuid();
        var jobDto = new JobDto(userId, Guid.NewGuid(), JobStatus.Open, 10, 10, "path");
        createJobUseCaseMock.Setup(useCase => useCase.ExecuteAsync(It.IsAny<CreateJobDto>())).ReturnsAsync(jobDto);
        var service = new JobManagerService(createJobUseCaseMock.Object, null, null);

        // Act
        var result = await service.CreateJobAsync(new CreateJobDto(userId, 10));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(jobDto.UserId, result.UserId);
        Assert.Equal(jobDto.JobId, result.JobId);
    }

    [Fact]
    public async Task ListJobsAsync_ReturnsJobs_WhenJobsExist()
    {
        // Arrange
        var getUserJobsUseCaseMock = new Mock<IUseCase<Guid, IReadOnlyList<JobDto>>>();
        var jobs = new List<JobDto>
        {
            new(Guid.NewGuid(), Guid.NewGuid(), JobStatus.Open, 10, 10, "path"),
            new(Guid.NewGuid(), Guid.NewGuid(), JobStatus.Open, 10, 10, "path")
        };
        getUserJobsUseCaseMock.Setup(useCase => useCase.ExecuteAsync(It.IsAny<Guid>())).ReturnsAsync(jobs);
        var service = new JobManagerService(null, getUserJobsUseCaseMock.Object, null);

        // Act
        var result = await service.ListJobsAsync(Guid.NewGuid());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetOneAsync_ReturnsJobDto_WhenJobExists()
    {
        // Arrange
        var getJobDetailUseCaseMock = new Mock<IUseCase<GetJobDetailDto, JobDto?>>();
        var jobDto = new JobDto(Guid.NewGuid(), Guid.NewGuid(), JobStatus.Open, 10, 10, "path");
        getJobDetailUseCaseMock.Setup(useCase => useCase.ExecuteAsync(It.IsAny<GetJobDetailDto>())).ReturnsAsync(jobDto);
        var service = new JobManagerService(null, null, getJobDetailUseCaseMock.Object);

        // Act
        var result = await service.GetOneAsync(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(jobDto.UserId, result.UserId);
        Assert.Equal(jobDto.JobId, result.JobId);
    }

    [Fact]
    public async Task GetOneAsync_ReturnsNull_WhenJobDoesNotExist()
    {
        // Arrange
        var getJobDetailUseCaseMock = new Mock<IUseCase<GetJobDetailDto, JobDto?>>();
        getJobDetailUseCaseMock.Setup(useCase => useCase.ExecuteAsync(It.IsAny<GetJobDetailDto>())).ReturnsAsync((JobDto?)null);
        var service = new JobManagerService(null, null, getJobDetailUseCaseMock.Object);

        // Act
        var result = await service.GetOneAsync(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }
}