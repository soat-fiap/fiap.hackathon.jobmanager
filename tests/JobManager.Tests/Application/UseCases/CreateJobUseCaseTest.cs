using JetBrains.Annotations;
using JobManager.Application;
using JobManager.Domain.Contracts;
using JobManager.Domain.Dto;
using JobManager.Domain.Entities;
using JobManager.Domain.ValueObjects;
using Moq;

namespace JobManager.Tests.Application.UseCases;

[TestSubject(typeof(CreateJobUseCase))]
public class CreateJobUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_CreatesJobSuccessfully()
    {
        // Arrange
        var jobRepositoryMock = new Mock<IJobRepository>();
        var createJobDto = new CreateJobDto(Guid.NewGuid(), 5);
        var useCase = new CreateJobUseCase(jobRepositoryMock.Object);

        // Act
        var result = await useCase.ExecuteAsync(createJobDto);

        // Assert
        jobRepositoryMock.Verify(repo => repo.SaveAsync(It.IsAny<Job>()), Times.Once);
        Assert.Equal(createJobDto.UserId, result.UserId);
        Assert.Equal(createJobDto.SnapshotsCount, result.Snapshots);
    }

    [Fact]
    public async Task ExecuteAsync_ThrowsException_WhenRepositoryFails()
    {
        // Arrange
        var jobRepositoryMock = new Mock<IJobRepository>();
        jobRepositoryMock.Setup(repo => repo.SaveAsync(It.IsAny<Job>()))
            .ThrowsAsync(new Exception("Repository failure"));
        var createJobDto = new CreateJobDto(Guid.NewGuid(), 5);
        var useCase = new CreateJobUseCase(jobRepositoryMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => useCase.ExecuteAsync(createJobDto));
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsJobDtoWithCorrectStatus()
    {
        // Arrange
        var jobRepositoryMock = new Mock<IJobRepository>();
        var createJobDto = new CreateJobDto(Guid.NewGuid(), 5);
        var useCase = new CreateJobUseCase(jobRepositoryMock.Object);

        // Act
        var result = await useCase.ExecuteAsync(createJobDto);

        // Assert
        Assert.Equal(JobStatus.Open, result.Status);
    }
}