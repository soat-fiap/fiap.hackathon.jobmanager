using System.Security.Claims;
using JetBrains.Annotations;
using JobManager.Api.Controllers;
using JobManager.Api.Model;
using JobManager.Controllers.Contracts;
using JobManager.Domain.Dto;
using JobManager.Domain.ValueObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace JobManager.Tests.Api;

[TestSubject(typeof(JobController))]
public class JobControllerTest
{
    private readonly Mock<IJobManagerService> _jobManagerServiceMock;
    private readonly Mock<IFileUpload> _fileUploadMock;
    private readonly Mock<ILogger<JobController>> _loggerMock;
    private readonly JobController _controller;
    private static readonly Guid UserId = Guid.NewGuid();

    public JobControllerTest()
    {
        _loggerMock = new Mock<ILogger<JobController>>();
        _jobManagerServiceMock = new Mock<IJobManagerService>();
        _fileUploadMock = new Mock<IFileUpload>();

        _controller = new JobController(_loggerMock.Object, _jobManagerServiceMock.Object, _fileUploadMock.Object);

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = HttpContextMock.Create(UserId).Object
        };
    }

    [Fact]
    public async Task Create_ReturnsCreatedResult_WhenJobIsCreatedSuccessfully()
    {
        // Arrange
        var createJobRequest = new CreateJobRequest
        {
            Snapshots = 10
        };
        var createdJob = new JobDto(Guid.NewGuid(), Guid.NewGuid(), JobStatus.Open, 10, 0, "");

        _jobManagerServiceMock.Setup(service => service.CreateJobAsync(It.IsAny<CreateJobDto>()))
            .ReturnsAsync(createdJob);

        // Act
        var result = await _controller.Create(createJobRequest, CancellationToken.None);

        // Assert
        var actionResult = Assert.IsType<CreatedResult>(result);
        var returnValue = Assert.IsType<JobDto>(actionResult.Value);
        Assert.Equal(createdJob.UserId, returnValue.UserId);
    }

    [Fact]
    public async Task GetJobs_ReturnsOkResult_WithListOfJobs()
    {
        // Arrange
        var jobs = new List<JobDto> { new(Guid.NewGuid(), Guid.NewGuid(), JobStatus.Open, 10, 0, "") };

        _jobManagerServiceMock.Setup(service => service.ListJobsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(jobs);

        // Act
        var result = await _controller.GetJobs(CancellationToken.None);

        // Assert
        var actionResult = Assert.IsType<ActionResult<List<ListJobResponse>>>(result);
        Assert.NotEmpty((actionResult.Result as OkObjectResult).Value as List<ListJobResponse>);
    }

    [Fact]
    public async Task GetJob_ReturnsOkResult_WhenJobExists()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var job = new JobDto(Guid.NewGuid(), Guid.NewGuid(), JobStatus.Open, 10, 0, "");

        _jobManagerServiceMock.Setup(service => service.GetOneAsync(It.IsAny<Guid>(), jobId))
            .ReturnsAsync(job);

        // Act
        var result = await _controller.GetJob(jobId, CancellationToken.None);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ListJobResponse>>(result);
        Assert.Equal(job.JobId, ((actionResult.Result as OkObjectResult).Value as ListJobResponse).JobId);
    }

    [Fact]
    public async Task GetJob_ReturnsNotFoundResult_WhenJobDoesNotExist()
    {
        // Arrange
        var jobId = Guid.NewGuid();

        _jobManagerServiceMock.Setup(service => service.GetOneAsync(It.IsAny<Guid>(), jobId))
            .ReturnsAsync((JobDto)null);

        // Act
        var result = await _controller.GetJob(jobId, CancellationToken.None);

        // Assert
        Assert.IsType<ActionResult<ListJobResponse>>(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetUploadLink_ReturnsOkResult_WithUploadUrl()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var job = new JobDto(jobId, Guid.NewGuid(), JobStatus.Open, 10, 0, "");
        var uploadUrl = "http://example.com/upload";

        _jobManagerServiceMock.Setup(service => service.GetOneAsync(It.IsAny<Guid>(), jobId))
            .ReturnsAsync(job);
        _fileUploadMock.Setup(upload => upload.CreateUploadUrl(It.IsAny<JobDto>()))
            .ReturnsAsync(uploadUrl);

        // Act
        var result = await _controller.GetUploadLink(jobId, CancellationToken.None);

        // Assert
        var actionResult = Assert.IsType<ActionResult<UploadUrlResponse>>(result);
        Assert.Equal(uploadUrl, ((actionResult.Result as OkObjectResult).Value as UploadUrlResponse).Url);
    }

    [Fact]
    public async Task GetUploadLink_ReturnsNotFoundResult_WhenJobDoesNotExist()
    {
        // Arrange
        var jobId = Guid.NewGuid();

        _jobManagerServiceMock.Setup(service => service.GetOneAsync(It.IsAny<Guid>(), jobId))
            .ReturnsAsync((JobDto)null);

        // Act
        var result = await _controller.GetUploadLink(jobId, CancellationToken.None);

        // Assert
        Assert.IsType<ActionResult<UploadUrlResponse>>(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetDownloadLink_ReturnsOkResult_WithDownloadUrl()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var job = new JobDto(jobId, Guid.NewGuid(), JobStatus.Open, 10, 0, "");
        var downloadUrl = "http://example.com/download";

        _jobManagerServiceMock.Setup(service => service.GetOneAsync(It.IsAny<Guid>(), jobId))
            .ReturnsAsync(job);
        _fileUploadMock.Setup(upload => upload.CreateDownloadUrl(job))
            .ReturnsAsync(downloadUrl);

        // Act
        var result = await _controller.GetDownloadLink(jobId, CancellationToken.None);

        // Assert
        Assert.IsType<ActionResult<DownloadUrlResponse>>(result);
        Assert.Equal(downloadUrl, ((result.Result as OkObjectResult).Value as DownloadUrlResponse).Url);
    }

    [Fact]
    public async Task GetDownloadLink_ReturnsNotFoundResult_WhenJobDoesNotExist()
    {
        // Arrange
        var jobId = Guid.NewGuid();

        _jobManagerServiceMock.Setup(service => service.GetOneAsync(It.IsAny<Guid>(), jobId))
            .ReturnsAsync((JobDto)null);

        // Act
        var result = await _controller.GetDownloadLink(jobId, CancellationToken.None);

        // Assert
        Assert.IsType<ActionResult<DownloadUrlResponse>>(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }
}

static class HttpContextMock
{
    public static Mock<HttpContext> Create()
    {
        var context = new Mock<HttpContext>();
        var request = new Mock<HttpRequest>();
        var response = new Mock<HttpResponse>();
        var user = new Mock<ClaimsPrincipal>();
        var identity = new Mock<ClaimsIdentity>();

        context.Setup(ctx => ctx.Request).Returns(request.Object);
        context.Setup(ctx => ctx.Response).Returns(response.Object);
        context.Setup(ctx => ctx.User).Returns(user.Object);

        user.Setup(u => u.Identity).Returns(identity.Object);
        identity.Setup(i => i.IsAuthenticated).Returns(true);

        return context;
    }

    public static Mock<HttpContext> Create(Guid userId)
    {
        var context = new Mock<HttpContext>();
        var request = new Mock<HttpRequest>();
        var response = new Mock<HttpResponse>();
        var user = new Mock<ClaimsPrincipal>();
        var identity = new Mock<ClaimsIdentity>();
        identity.Setup(s => s.Claims).Returns(
        [
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        ]);

        context.Setup(ctx => ctx.Request).Returns(request.Object);
        context.Setup(ctx => ctx.Response).Returns(response.Object);
        context.Setup(ctx => ctx.User).Returns(user.Object);

        user.Setup(u => u.Identity).Returns(identity.Object);
        identity.Setup(i => i.IsAuthenticated).Returns(true);

        return context;
    }
}